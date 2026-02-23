using TechFlow.Domain.Common;

namespace TechFlow.Domain.Boards.Lists.Events;

public sealed class ListReorderedEvent : DomainEvent
{
    public Guid BoardId { get; }

    public ListReorderedEvent(Guid boardId)
    {
        BoardId = boardId;
    }
}