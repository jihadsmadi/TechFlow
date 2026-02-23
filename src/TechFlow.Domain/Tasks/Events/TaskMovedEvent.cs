using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Events;

public sealed class TaskMovedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid FromListId { get; }
    public Guid ToListId { get; }
    public Guid ProjectId { get; }

    public TaskMovedEvent(Guid taskId, Guid fromListId, Guid toListId, Guid projectId)
    {
        TaskId = taskId;
        FromListId = fromListId;
        ToListId = toListId;
        ProjectId = projectId;
    }
}
