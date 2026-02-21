namespace TechFlow.Domain.Common;

public abstract class DomainEvent 
{
    public DateTime OccurredOn { get; }
    public Guid EventId { get; }
}