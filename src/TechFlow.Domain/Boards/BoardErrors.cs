using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Boards;

public static class BoardErrors
{
    public static readonly Error ProjectIdRequired =
        Error.Validation("Board.ProjectIdRequired", "Project ID is required.");

    public static readonly Error NameRequired =
        Error.Validation("Board.NameRequired", "Board name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Board.NameTooLong", "Board name is too long.");

    public static readonly Error NotFound =
        Error.NotFound("Board.NotFound", "Board was not found.");

    public static readonly Error InvalidListOrder =
        Error.Validation("Board.InvalidListOrder", "One or more list IDs do not belong to this board.");
}