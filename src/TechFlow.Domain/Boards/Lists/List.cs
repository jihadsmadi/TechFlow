using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Boards;

/// <summary>
/// Represents a column on a board (e.g. "To Do", "In Progress", "Done").
/// Belongs to the Board aggregate — never created independently.
/// </summary>
public sealed class List : Entity
{
    public Guid BoardId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsCompletedList { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private List() { }

    private List(Guid id, Guid boardId, string name, string? color, int displayOrder, bool isDefault, bool isCompletedList)
        : base(id)
    {
        BoardId = boardId;
        Name = name;
        Color = color;
        DisplayOrder = displayOrder;
        IsDefault = isDefault;
        IsCompletedList = isCompletedList;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    internal static Result<List> Create(
        Guid boardId,
        string name,
        int displayOrder,
        bool isDefault = false,
        bool isCompletedList = false,
        string? color = null)
    {
        if (!IsValidId(boardId))
            return ListErrors.BoardIdRequired;

        if (string.IsNullOrWhiteSpace(name))
            return ListErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return ListErrors.NameTooLong;

        if (!IsValidDisplayOrder(displayOrder))
            return ListErrors.InvalidDisplayOrder;

        return new List(
            id: Guid.NewGuid(),
            boardId: boardId,
            name: name.Trim(),
            color: color?.Trim(),
            displayOrder: displayOrder,
            isDefault: isDefault,
            isCompletedList: isCompletedList
        );
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ListErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return ListErrors.NameTooLong;

        Name = name.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Updated> SetColor(string? color)
    {
        Color = color?.Trim();
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    /// <summary>
    /// Called by Board when reordering columns.
    /// Only Board can reorder — lists don't reorder themselves.
    /// </summary>
    internal void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private static bool IsValidDisplayOrder(int order) => order >= 0;
}