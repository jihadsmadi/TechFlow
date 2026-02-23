using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Events;

public sealed class TaskCompletedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid ProjectId { get; }
    public Guid CompanyId { get; }

    public TaskCompletedEvent(Guid taskId, Guid projectId, Guid companyId)
    {
        TaskId = taskId;
        ProjectId = projectId;
        CompanyId = companyId;
    }
}
