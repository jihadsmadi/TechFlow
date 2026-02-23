using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Events;

public sealed class TaskUnassignedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid UserId { get; }

    public TaskUnassignedEvent(Guid taskId, Guid userId)
    {
        TaskId = taskId;
        UserId = userId;
    }
}
