using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Boards;

public static class ListErrors
{
    public static readonly Error BoardIdRequired =
        Error.Validation("List.BoardIdRequired", "Board ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("List.NameRequired", "List name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("List.NameTooLong", "List name is too long.");

    public static readonly Error InvalidDisplayOrder =
        Error.Validation("List.InvalidDisplayOrder", "Display order must be zero or greater.");

    public static readonly Error NotFound =
        Error.NotFound("List.NotFound", "List was not found.");

    public static readonly Error CannotDeleteDefault =
        Error.Forbidden("List.CannotDeleteDefault", "Default lists cannot be deleted.");

    public static readonly Error DuplicateName =
        Error.Conflict("List.DuplicateName", "A list with this name already exists on this board.");
}