using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.TaskAssignments;

/// <summary>
/// Records that a specific user is assigned to a specific task.
/// A task can have multiple assignees via this junction entity.
/// </summary>
public sealed class TaskAssignment : Entity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AssignedByUserId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }

    private TaskAssignment() { }

    private TaskAssignment(Guid id, Guid taskId, Guid userId, Guid assignedByUserId)
        : base(id)
    {
        TaskId = taskId;
        UserId = userId;
        AssignedByUserId = assignedByUserId;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    internal static Result<TaskAssignment> Create(Guid taskId, Guid userId, Guid assignedByUserId)
    {
        if (!IsValidId(taskId))
            return TaskAssignmentErrors.TaskIdRequired;

        if (!IsValidId(userId))
            return TaskAssignmentErrors.UserIdRequired;

        if (!IsValidId(assignedByUserId))
            return TaskAssignmentErrors.AssignedByRequired;

        return new TaskAssignment(Guid.NewGuid(), taskId, userId, assignedByUserId);
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}