using TechFlow.Domain.Common;

namespace TechFlow.Domain.Boards.Events;

public sealed class BoardCreatedEvent : DomainEvent
{
    public Guid BoardId { get; }
    public Guid ProjectId { get; }

    public BoardCreatedEvent(Guid boardId, Guid projectId)
    {
        BoardId = boardId;
        ProjectId = projectId;
    }
}
