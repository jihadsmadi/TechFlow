using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.TaskAssignments;

public static class TaskAssignmentErrors
{
    public static readonly Error TaskIdRequired =
        Error.Validation("TaskAssignment.TaskIdRequired", "Task ID is required.");

    public static readonly Error UserIdRequired =
        Error.Validation("TaskAssignment.UserIdRequired", "User ID is required.");

    public static readonly Error AssignedByRequired =
        Error.Validation("TaskAssignment.AssignedByRequired", "Assigned by user ID is required.");
}
