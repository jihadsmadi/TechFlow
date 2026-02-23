using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Events;

public sealed class TaskAssignedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid ProjectId { get; }

    public TaskAssignedEvent(Guid taskId, Guid userId, Guid projectId)
    {
        TaskId = taskId;
        UserId = userId;
        ProjectId = projectId;
    }
}
