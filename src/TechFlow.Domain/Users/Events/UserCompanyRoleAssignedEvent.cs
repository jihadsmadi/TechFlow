using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserCompanyRoleAssignedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid RoleId { get; }

    public UserCompanyRoleAssignedEvent(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}