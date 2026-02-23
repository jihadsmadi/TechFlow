using TechFlow.Domain.Common;

namespace TechFlow.Domain.Users.Events;

public sealed class UserActivatedEvent : DomainEvent
{
    public Guid UserId { get; }

    public UserActivatedEvent(Guid userId)
    {
        UserId = userId;
    }
}
