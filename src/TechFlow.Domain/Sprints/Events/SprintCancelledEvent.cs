using TechFlow.Domain.Common;

namespace TechFlow.Domain.Sprints.Events;

public sealed class SprintCancelledEvent : DomainEvent
{
    public Guid SprintId { get; }
    public Guid ProjectId { get; }

    public SprintCancelledEvent(Guid sprintId, Guid projectId)
    {
        SprintId = sprintId;
        ProjectId = projectId;
    }
}
