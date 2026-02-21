
using System.ComponentModel.DataAnnotations.Schema;

namespace TechFlow.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; }

    private readonly List<DomainEvent> _domainEvents = [];

    [NotMapped]
    public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected Entity()
    { }

    protected Entity(Guid id)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
            return false;
        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var other = (Entity)obj;
        return Id == other.Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
    public void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
        var eventType = domainEvent.GetType().Name;
    }

    public void RemoveDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}