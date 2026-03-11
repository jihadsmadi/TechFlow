using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserProjectRoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid ProjectId { get; }
    public Guid RoleId { get; }

    public UserProjectRoleAssignedEvent(Guid userId,Guid projectId, Guid roleId)
    {
        UserId = userId;
        ProjectId = projectId;
        RoleId = roleId;
    }
}