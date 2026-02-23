using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserRoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public Guid? ProjectId { get; }

    public UserRoleAssignedEvent(Guid userId, Guid roleId, Guid? projectId)
    {
        UserId = userId;
        RoleId = roleId;
        ProjectId = projectId;
    }
}