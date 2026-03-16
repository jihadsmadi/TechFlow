using MediatR;
using TechFlow.Application.Common.Events;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Tasks.Events;

namespace TechFlow.Application.Features.Tasks.EventsHandler;

/// <summary>
/// When a task is moved to a list with IsCompletedList = true → auto-complete it.
/// When a task is moved OUT of a completed list → reopen it.
/// </summary>
public sealed class TaskMovedEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<DomainEventNotification<TaskMovedEvent>>
{
    public async Task Handle(
        DomainEventNotification<TaskMovedEvent> notification,
        CancellationToken ct)
    {
        var domainEvent = notification.Event;

        // load the new list to check if it is a completion list
        var newList = await unitOfWork.Boards.GetListByIdAsync(domainEvent.ToListId, ct);
        if (newList is null) return;

        var task = await unitOfWork.Tasks.GetByIdAsync(domainEvent.TaskId, ct);
        if (task is null) return;

        if (newList.IsCompletedList && !task.IsCompleted)
            task.Complete();
        else if (!newList.IsCompletedList && task.IsCompleted)
            task.Reopen();

        await unitOfWork.SaveChangesAsync(ct);
    }
}