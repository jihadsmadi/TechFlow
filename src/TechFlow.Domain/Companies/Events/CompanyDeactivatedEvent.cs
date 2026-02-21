using TechFlow.Domain.Common;

namespace TechFlow.Domain.Companies.Events;

public class CompanyDeactivatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public CompanyDeactivatedEvent(Guid companyId)
    {
           this.CompanyId = companyId;
    }
}
