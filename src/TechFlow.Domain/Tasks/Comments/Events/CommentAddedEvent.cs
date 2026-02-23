using TechFlow.Domain.Common;

namespace TechFlow.Domain.Tasks.Comments.Events;

public sealed class CommentAddedEvent : DomainEvent
{
    public Guid TaskId { get; }
    public Guid UserId { get; }
    public Guid ProjectId { get; }

    public CommentAddedEvent(Guid taskId, Guid userId, Guid projectId)
    {
        TaskId = taskId;
        UserId = userId;
        ProjectId = projectId;
    }
}