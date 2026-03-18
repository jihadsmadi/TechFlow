using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Sprints;

public static class SprintErrors
{
    public static readonly Error SprintIdRequired =
        Error.Validation("Sprint.SprintIdRequired", "Sprint ID is required.");

    public static readonly Error TaskIdRequired =
        Error.Validation("Sprint.TaskIdRequired", "Task ID is required.");

    public static readonly Error ProjectIdRequired =
        Error.Validation("Sprint.ProjectIdRequired", "Project ID is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Sprint.NameTooLong", $"Sprint name must not exceed {TechFlowConstants.Validation.MaxNameLength} characters.");

    public static readonly Error InvalidDates =
        Error.Validation("Sprint.InvalidDates", "End date must be after start date.");

    public static readonly Error NotFound =
        Error.NotFound("Sprint.NotFound", "Sprint was not found.");

    public static readonly Error NotPlanned =
        Error.Conflict("Sprint.NotPlanned", "Only a planned sprint can be started.");

    public static readonly Error NotActive =
        Error.Conflict("Sprint.NotActive", "Only an active sprint can be ended.");

    public static readonly Error AlreadyActive =
        Error.Conflict("Sprint.AlreadyActive", "A sprint is already active for this project.");

    public static readonly Error AlreadyCancelled =
        Error.Conflict("Sprint.AlreadyCancelled", "Sprint is already cancelled.");

    public static readonly Error AlreadyCompleted =
        Error.Conflict("Sprint.AlreadyCompleted", "Sprint is already completed.");

    public static readonly Error TaskAlreadyInSprint =
        Error.Conflict("Sprint.TaskAlreadyInSprint", "Task is already in this sprint.");

    public static readonly Error TaskNotInSprint =
        Error.NotFound("Sprint.TaskNotInSprint", "Task is not in this sprint.");

    public static readonly Error CannotModifyActiveSprint =
        Error.Conflict("Sprint.CannotModifyActiveSprint", "Cannot add or remove tasks from a locked active sprint.");

    public static readonly Error InvalidIncompleteTasksAction =
        Error.Validation("Sprint.InvalidIncompleteTasksAction", "Invalid incomplete tasks action.");
}
