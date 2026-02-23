using TechFlow.Domain.Common;

namespace TechFlow.Domain.Projects.Events;

public sealed class ProjectMemberAddedEvent : DomainEvent
{
    public Guid ProjectId { get; }
    public Guid UserId { get; }

    public ProjectMemberAddedEvent(Guid projectId, Guid userId)
    {
        ProjectId = projectId;
        UserId = userId;
    }
}
