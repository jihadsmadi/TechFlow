using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserDeactivatedEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid CompanyId { get; }

    public UserDeactivatedEvent(Guid userId, Guid companyId)
    {
        UserId = userId;
        CompanyId = companyId;
    }
}
