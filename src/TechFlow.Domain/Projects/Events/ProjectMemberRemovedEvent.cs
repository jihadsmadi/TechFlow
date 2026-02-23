using TechFlow.Domain.Common;

namespace TechFlow.Domain.Projects.Events;

public sealed class ProjectMemberRemovedEvent : DomainEvent
{
    public Guid ProjectId { get; }
    public Guid UserId { get; }

    public ProjectMemberRemovedEvent(Guid projectId, Guid userId)
    {
        ProjectId = projectId;
        UserId = userId;
    }
}