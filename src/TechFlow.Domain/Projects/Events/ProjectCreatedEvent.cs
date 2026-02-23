using TechFlow.Domain.Common;

namespace TechFlow.Domain.Projects.Events;

public sealed class ProjectCreatedEvent : DomainEvent
{
    public Guid ProjectId { get; }
    public Guid CompanyId { get; }
    public string Name { get; }

    public ProjectCreatedEvent(Guid projectId, Guid companyId, string name)
    {
        ProjectId = projectId;
        CompanyId = companyId;
        Name = name;
    }
}
