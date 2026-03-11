using TechFlow.Domain.Boards.Events;
using TechFlow.Domain.Boards.Lists.Events;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Tasks.Events;

namespace TechFlow.Domain.Boards;

/// <summary>
/// Aggregate root for the board and its lists.
/// 1-to-1 with Project — one project has exactly one board.
/// Board is the only way to create and manage Lists.
/// DisplayOrder uses double for fractional indexing.
/// </summary>
public sealed class Board : AuditableEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private readonly List<List> _lists = [];
    public IReadOnlyList<List> Lists => _lists.AsReadOnly();

    private Board() { }

    private Board(Guid id, Guid projectId, string name) : base(id)
    {
        ProjectId = projectId;
        Name = name;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a board and auto-creates its default lists.
    /// Default lists use fractional display orders: 0.0, 1.0, 2.0, 3.0
    /// giving room for future insertions between them.
    /// </summary>
    public static Result<Board> Create(
        Guid projectId,
        string name,
        IEnumerable<string> defaultListNames)
    {
        if (!IsValidId(projectId))
            return BoardErrors.ProjectIdRequired;

        if (string.IsNullOrWhiteSpace(name))
            return BoardErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return BoardErrors.NameTooLong;

        var board = new Board(
            id: Guid.NewGuid(),
            projectId: projectId,
            name: name.Trim());

        var namesList = defaultListNames.ToList();
        var totalLists = namesList.Count;

        for (var i = 0; i < totalLists; i++)
        {
            var isLast = i == totalLists - 1;
            var isCompletedList = isLast; // last list = "Done"

            // fractional starting positions: 0.0, 1.0, 2.0, 3.0 ...
            // leaves room for insertions between whole numbers
            var displayOrder = (double)i;

            var listResult = List.Create(
                boardId: board.Id,
                name: namesList[i],
                displayOrder: displayOrder,
                isDefault: true,
                isCompletedList: isCompletedList);

            if (listResult.IsFailure)
                return listResult.TopError;

            board._lists.Add(listResult.Value);
        }

        board.AddDomainEvent(new BoardCreatedEvent(board.Id, board.ProjectId));

        return board;
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return BoardErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return BoardErrors.NameTooLong;

        Name = name.Trim();
        return Result.Updated;
    }

    public Result<List> AddList(string name, string? color = null)
    {
        if (HasListWithName(name))
            return ListErrors.DuplicateName;

        // new list appended after all existing lists
        // e.g. if last list is at 3.0, new one goes to 4.0
        var maxOrder = _lists.Count > 0 ? _lists.Max(l => l.DisplayOrder) : -1.0;
        var displayOrder = maxOrder + 1.0;

        var listResult = List.Create(
            boardId: Id,
            name: name,
            displayOrder: displayOrder,
            color: color);

        if (listResult.IsFailure)
            return listResult.TopError;

        _lists.Add(listResult.Value);

        AddDomainEvent(new ListCreatedEvent(listResult.Value.Id, Id, listResult.Value.Name));

        return listResult.Value;
    }

    public Result<Updated> RemoveList(Guid listId)
    {
        var list = FindList(listId);
        if (list is null)
            return ListErrors.NotFound;

        if (list.IsDefault)
            return ListErrors.CannotDeleteDefault;

        _lists.Remove(list);

        // no need to reorder — fractional indexing means gaps are fine
        // remaining orders are still valid relative to each other

        return Result.Updated;
    }

    public Result<Updated> ReorderLists(IEnumerable<Guid> orderedListIds)
    {
        var ids = orderedListIds.ToList();

        if (!AreValidListIds(ids))
            return BoardErrors.InvalidListOrder;

        // assign whole number positions: 0.0, 1.0, 2.0 ...
        // resets fractional drift after many reorders
        for (var i = 0; i < ids.Count; i++)
        {
            var list = FindList(ids[i])!;
            list.SetDisplayOrder(i);
        }

        AddDomainEvent(new ListReorderedEvent(Id));

        return Result.Updated;
    }

    public Result<Updated> RenameList(Guid listId, string name)
    {
        var list = FindList(listId);
        if (list is null)
            return ListErrors.NotFound;

        if (_lists.Any(l => l.Id != listId && l.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase)))
            return ListErrors.DuplicateName;

        return list.Rename(name);
    }

    public Result<Updated> SetListColor(Guid listId, string? color)
    {
        var list = FindList(listId);
        if (list is null)
            return ListErrors.NotFound;

        return list.SetColor(color);
    }

    public List? GetCompletedList() =>
        _lists.FirstOrDefault(l => l.IsCompletedList);

    // ── Private Helpers ────────────────────────────────────────────────────────

    private List? FindList(Guid listId) =>
        _lists.FirstOrDefault(l => l.Id == listId);

    private bool HasListWithName(string name) =>
        _lists.Any(l => l.Name.Equals(name.Trim(), StringComparison.OrdinalIgnoreCase));

    private bool AreValidListIds(IEnumerable<Guid> ids) =>
        ids.All(id => _lists.Any(l => l.Id == id));

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}