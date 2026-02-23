using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserRegisteredEvent : DomainEvent
{
    public Guid UserId { get; }
    public Guid CompanyId { get; }
    public string Email { get; }

    public UserRegisteredEvent(Guid userId, Guid companyId, string email)
    {
        UserId = userId;
        CompanyId = companyId;
        Email = email;
    }
}
