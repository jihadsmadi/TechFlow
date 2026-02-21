
using TechFlow.Domain.Common;

namespace TechFlow.Domain.Companies.Events;

public class CompanyCreatedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string Name { get; }
    public string Slug { get; }

    public CompanyCreatedEvent(Guid companyId, string name, string slug)
    {
        CompanyId = companyId;
        Name = name;
        Slug = slug;
    }
}
