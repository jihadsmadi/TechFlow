using TechFlow.Domain.Common;

namespace TechFlow.Domain.Projects.Events;

public sealed class ProjectArchivedEvent : DomainEvent
{
    public Guid ProjectId { get; }
    public Guid CompanyId { get; }

    public ProjectArchivedEvent(Guid projectId, Guid companyId)
    {
        ProjectId = projectId;
        CompanyId = companyId;
    }
}
