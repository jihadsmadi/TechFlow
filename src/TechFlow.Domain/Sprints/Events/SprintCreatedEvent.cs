using TechFlow.Domain.Common;

namespace TechFlow.Domain.Sprints.Events;

public sealed class SprintCreatedEvent : DomainEvent
{
    public Guid SprintId { get; }
    public Guid ProjectId { get; }
    public int SprintNumber { get; }

    public SprintCreatedEvent(Guid sprintId, Guid projectId, int sprintNumber)
    {
        SprintId = sprintId;
        ProjectId = projectId;
        SprintNumber = sprintNumber;
    }
}
