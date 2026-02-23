using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Events;

public sealed class TaskCreatedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid ProjectId { get; }
    public Guid CompanyId { get; }
    public string Title { get; }

    public TaskCreatedEvent(Guid taskId, Guid projectId, Guid companyId, string title)
    {
        TaskId = taskId;
        ProjectId = projectId;
        CompanyId = companyId;
        Title = title;
    }
}
