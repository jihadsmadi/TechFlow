using TechFlow.Domain.Common;

namespace TechFlow.Domain.Sprints.Events;

public sealed class SprintStartedEvent : DomainEvent
{
    public Guid SprintId { get; }
    public Guid ProjectId { get; }

    public SprintStartedEvent(Guid sprintId, Guid projectId)
    {
        SprintId = sprintId;
        ProjectId = projectId;
    }
}
