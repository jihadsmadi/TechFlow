using TechFlow.Domain.Common;

namespace TechFlow.Domain.Sprints.Events;

public sealed class SprintEndedEvent : DomainEvent
{
    public Guid SprintId { get; }
    public Guid ProjectId { get; }
    public string IncompleteTasksAction { get; }

    public SprintEndedEvent(Guid sprintId, Guid projectId, string incompleteTasksAction)
    {
        SprintId = sprintId;
        ProjectId = projectId;
        IncompleteTasksAction = incompleteTasksAction;
    }
}
