using TechFlow.Domain.Common;

namespace TechFlow.Domain.Boards.Lists.Events;

public sealed class ListCreatedEvent : DomainEvent
{
    public Guid ListId { get; }
    public Guid BoardId { get; }
    public string Name { get; }

    public ListCreatedEvent(Guid listId, Guid boardId, string name)
    {
        ListId = listId;
        BoardId = boardId;
        Name = name;
    }
}
