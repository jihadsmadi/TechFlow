using MediatR;
using TechFlow.Application.Common.Events;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Projects.ProjectSettings;
using TechFlow.Domain.Sprints.Events;
using TechFlow.Domain.Sprints.ValueObjects;

namespace TechFlow.Application.Features.Sprints.EventHandlers;

/// <summary>
/// Fires after a sprint is ended.
/// Handles what happens to incomplete tasks based on PM's chosen action:
///   MoveToBacklog      — remove tasks from sprint, leave in their current list
///   MoveToNextSprint   — move tasks to the next planned sprint if one exists
///   LeaveInPlace       — do nothing, tasks stay in sprint and current list
/// </summary>
public sealed class SprintEndedEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<DomainEventNotification<SprintEndedEvent>>
{
    public async Task Handle(
        DomainEventNotification<SprintEndedEvent> notification,
        CancellationToken ct)
    {
        var domainEvent = notification.Event;

        var sprint = await unitOfWork.Sprints.GetByIdWithItemsAsync(domainEvent.SprintId, ct);
        if (sprint is null) return;

        // load all tasks in this sprint
        var taskIds = sprint.Items.Select(i => i.TaskId).ToList();
        if (taskIds.Count == 0) return;

        var tasks = await unitOfWork.Tasks.GetByIdsAsync(taskIds, ct);

        // filter to only incomplete tasks
        var incompleteTasks = tasks.Where(t => !t.IsCompleted).ToList();
        if (incompleteTasks.Count == 0) return;

        switch (domainEvent.IncompleteTasksAction)
        {
            case IncompleteTasksActionType.MoveToBacklog:
                // remove incomplete tasks from this sprint
                // tasks stay in their current list — backlog is just "not in a sprint"
                foreach (var task in incompleteTasks)
                    sprint.RemoveTask(task.Id);
                break;

            case IncompleteTasksActionType.MoveToNextSprint:
                // find the next planned sprint for this project
                var nextSprint = await unitOfWork.Sprints
                    .GetNextPlannedSprintAsync(domainEvent.ProjectId, ct);

                if (nextSprint is not null)
                {
                    // load next sprint with its items to check for duplicates
                    var nextSprintWithItems = await unitOfWork.Sprints
                        .GetByIdWithItemsAsync(nextSprint.Id, ct);

                    if (nextSprintWithItems is not null)
                    {
                        foreach (var task in incompleteTasks)
                        {
                            // only add if not already in next sprint
                            if (!nextSprintWithItems.HasTask(task.Id))
                            {
                                var addResult = nextSprintWithItems.AddTask(task.Id);
                                if (addResult.IsSuccess)
                                    unitOfWork.Sprints.MarkSprintItemAsAdded(addResult.Value);
                            }
                        }
                    }
                }
                // if no next sprint exists — tasks naturally become backlog
                break;

            case IncompleteTasksActionType.LeaveInPlace:
                // do nothing — tasks stay in sprint and their current list
                break;
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}
