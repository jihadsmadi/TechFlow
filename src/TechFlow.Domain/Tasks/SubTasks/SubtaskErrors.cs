using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.Subtasks;

public static class SubtaskErrors
{
    public static readonly Error TaskIdRequired =
        Error.Validation("Subtask.TaskIdRequired", "Task ID is required.");

    public static readonly Error CreatedByRequired =
        Error.Validation("Subtask.CreatedByRequired", "Creator ID is required.");

    public static readonly Error TitleRequired =
        Error.Validation("Subtask.TitleRequired", "Subtask title is required.");

    public static readonly Error TitleTooLong =
        Error.Validation("Subtask.TitleTooLong", "Subtask title is too long.");

    public static readonly Error NotFound =
        Error.NotFound("Subtask.NotFound", "Subtask was not found.");

    public static readonly Error AlreadyCompleted =
        Error.Conflict("Subtask.AlreadyCompleted", "Subtask is already completed.");

    public static readonly Error NotCompleted =
        Error.Conflict("Subtask.NotCompleted", "Subtask is not completed.");
}