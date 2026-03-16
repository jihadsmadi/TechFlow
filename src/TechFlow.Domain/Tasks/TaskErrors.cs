using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks;

public static class TaskErrors
{
    public static readonly Error ListIdRequired =
        Error.Validation("Task.ListIdRequired", "List ID is required.");

    public static readonly Error CompanyIdRequired =
        Error.Validation("Task.CompanyIdRequired", "Company ID is required.");

    public static readonly Error ProjectIdRequired =
        Error.Validation("Task.ProjectIdRequired", "Project ID is required.");

    public static readonly Error CreatedByRequired =
        Error.Validation("Task.CreatedByRequired", "Created by user ID is required.");

    public static readonly Error TitleRequired =
        Error.Validation("Task.TitleRequired", "Task title is required.");

    public static readonly Error TitleTooLong =
        Error.Validation("Task.TitleTooLong", "Task title must not exceed 100 characters.");

    public static Error InvalidPriority(string value) =>
        Error.Validation("Task.InvalidPriority", $"'{value}' is not a valid priority.");

    public static Error InvalidType(string value) =>
        Error.Validation("Task.InvalidType", $"'{value}' is not a valid task type.");

    public static readonly Error InvalidEstimate =
        Error.Validation("Task.InvalidEstimate", "Estimated minutes must be greater than zero.");

    public static readonly Error InvalidActualTime =
        Error.Validation("Task.InvalidActualTime", "Actual minutes must be greater than zero.");

    public static readonly Error NotFound =
        Error.NotFound("Task.NotFound", "Task was not found.");

    public static readonly Error AlreadyCompleted =
        Error.Conflict("Task.AlreadyCompleted", "Task is already completed.");

    public static readonly Error NotCompleted =
        Error.Conflict("Task.NotCompleted", "Task is not completed.");

    public static readonly Error AlreadyAssigned =
        Error.Conflict("Task.AlreadyAssigned", "User is already assigned to this task.");

    public static readonly Error AssignmentNotFound =
        Error.NotFound("Task.AssignmentNotFound", "Assignment was not found.");

    public static readonly Error SubtasksNotAllowed =
        Error.Conflict("Task.SubtasksNotAllowed", "Subtasks are not allowed for this task.");
    public static readonly Error EstimateRequired = 
        Error.Validation("Task.EstimateRequired", "Estimated minutes are required for this task.");
    public static readonly Error ListNotFound =
        Error.NotFound("Task.ListNotFound", "Target list was not found.");
    public static readonly Error ListNotBelongToProject =
        Error.Validation("Task.ListNotBelongToProject", "Target list does not belong to the project.");
}