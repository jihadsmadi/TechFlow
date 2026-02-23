using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks;

public static class TaskErrors
{
    // IDs
    public static readonly Error ListIdRequired =
        Error.Validation("Task.ListIdRequired", "List ID is required.");

    public static readonly Error CompanyIdRequired =
        Error.Validation("Task.CompanyIdRequired", "Company ID is required.");

    public static readonly Error ProjectIdRequired =
        Error.Validation("Task.ProjectIdRequired", "Project ID is required.");

    public static readonly Error CreatedByRequired =
        Error.Validation("Task.CreatedByRequired", "Created by user ID is required.");

    // Title
    public static readonly Error TitleRequired =
        Error.Validation("Task.TitleRequired", "Task title is required.");

    public static readonly Error TitleTooLong =
        Error.Validation("Task.TitleTooLong", "Task title is too long.");

    // Priority + Type
    public static Error InvalidPriority(string priority) =>
        Error.Validation("Task.InvalidPriority",
            $"'{priority}' is not valid. Valid priorities: Low, Medium, High, Critical.");

    public static Error InvalidType(string type) =>
        Error.Validation("Task.InvalidType",
            $"'{type}' is not valid. Valid types: Bug, Feature, Research, Chore.");

    // Estimates
    public static readonly Error InvalidEstimate =
        Error.Validation("Task.InvalidEstimate", "Estimated minutes must be greater than zero.");

    public static readonly Error InvalidActualTime =
        Error.Validation("Task.InvalidActualTime", "Actual minutes must be greater than zero.");

    // Subtasks
    public static readonly Error SubtasksNotAllowed =
        Error.Forbidden("Task.SubtasksNotAllowed", "Subtasks are not enabled for this project.");

    public static readonly Error CannotNestSubtask =
        Error.Validation("Task.CannotNestSubtask", "A subtask cannot have its own subtasks.");

    // State
    public static readonly Error AlreadyCompleted =
        Error.Conflict("Task.AlreadyCompleted", "Task is already completed.");

    public static readonly Error NotCompleted =
        Error.Conflict("Task.NotCompleted", "Task is not completed.");

    public static readonly Error NotFound =
        Error.NotFound("Task.NotFound", "Task was not found.");

    // Assignment
    public static readonly Error AlreadyAssigned =
        Error.Conflict("Task.AlreadyAssigned", "User is already assigned to this task.");

    public static readonly Error AssignmentNotFound =
        Error.NotFound("Task.AssignmentNotFound", "Assignment was not found.");
}
