using System.Collections.Generic;
using System.Xml.Linq;
using TechFlow.Domain.Boards.Events;
using TechFlow.Domain.Boards.Lists.Events;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Boards;

/// <summary>
/// Aggregate root for the board and its lists.
/// 1-to-1 with Project — one project has exactly one board.
/// Board is the only way to create and manage Lists.
/// </summary>
public sealed class Board : AuditableEntity
{
    public Guid ProjectId { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private readonly List<List> _lists = [];
    public IReadOnlyList<List> Lists => _lists.AsReadOnly();

    private Board() { }

    private Board(Guid id, Guid projectId, string name)
        : base(id)
    {
        ProjectId = projectId;
        Name = name;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a board and auto-creates its default lists from the project settings.
    /// Called by the application layer when a Project is created.
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
            name: name.Trim()
        );

        // Auto-create default lists from project settings
        var namesList = defaultListNames.ToList();
        var totalLists = namesList.Count;

        for (var i = 0; i < totalLists; i++)
        {
            var isLast = i == totalLists - 1;
            var isCompletedList = isLast;   // last list = "Done" = completion column

            var listResult = List.Create(
                boardId: board.Id,
                name: namesList[i],
                displayOrder: i,
                isDefault: true,
                isCompletedList: isCompletedList
            );

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

        var displayOrder = _lists.Count;  // next position after existing lists

        var listResult = List.Create(
            boardId: Id,
            name: name,
            displayOrder: displayOrder,
            color: color
        );

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

        // Reorder remaining lists to fill the gap
        ReorderLists();

        return Result.Updated;
    }

    public Result<Updated> ReorderLists(IEnumerable<Guid> orderedListIds)
    {
        var ids = orderedListIds.ToList();

        // Make sure all provided IDs exist on this board
        if (!AreValidListIds(ids))
            return BoardErrors.InvalidListOrder;

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

        // Check duplicate name — but exclude the list being renamed
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

    private void ReorderLists()
    {
        var ordered = _lists.OrderBy(l => l.DisplayOrder).ToList();
        for (var i = 0; i < ordered.Count; i++)
            ordered[i].SetDisplayOrder(i);
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}