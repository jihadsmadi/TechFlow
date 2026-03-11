using MediatR;
using TechFlow.Domain.Common;

namespace TechFlow.Application.Common.Events;

public sealed class DomainEventNotification<TEvent> : INotification
    where TEvent : DomainEvent
{
    public TEvent Event { get; }
    public DomainEventNotification(TEvent @event) => Event = @event;
}