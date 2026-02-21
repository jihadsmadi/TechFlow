namespace TechFlow.Domain.Common;

public abstract class DomainEvent 
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid EventId { get; } = Guid.NewGuid();
}