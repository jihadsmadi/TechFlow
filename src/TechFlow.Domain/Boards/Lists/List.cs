using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Boards;

public sealed class List : AuditableEntity
{
    public Guid BoardId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Color { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsDefault { get; private set; }
    public bool IsCompletedList { get; private set; }

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
    }

    // ── Factory

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

    // ── Business 

    public Result<Updated> Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ListErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return ListErrors.NameTooLong;

        Name = name.Trim();

        return Result.Updated;
    }

    public Result<Updated> SetColor(string? color)
    {
        Color = color?.Trim();

        return Result.Updated;
    }

    internal void SetDisplayOrder(int order)
    {
        DisplayOrder = order;
    }

    // ── Private Validation 

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private static bool IsValidDisplayOrder(int order) => order >= 0;
}