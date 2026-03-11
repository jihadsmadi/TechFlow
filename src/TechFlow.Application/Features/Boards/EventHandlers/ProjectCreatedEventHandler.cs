using MediatR;
using TechFlow.Application.Common.Events;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Boards;
using TechFlow.Domain.Projects.Events;

namespace TechFlow.Application.Features.Boards.EventHandlers;

public sealed class ProjectCreatedEventHandler(IUnitOfWork unitOfWork)
    : INotificationHandler<DomainEventNotification<ProjectCreatedEvent>>
{
    public async Task Handle(
        DomainEventNotification<ProjectCreatedEvent> notification,
        CancellationToken ct)
    {
        var domainEvent = notification.Event;

        var project = await unitOfWork.Projects.GetByIdAsync(domainEvent.ProjectId, ct);
        if (project is null) return;

        var defaultListNames = project.Settings.GetDefaultListNames();

        var boardResult = Board.Create(
            projectId: domainEvent.ProjectId,
            name: domainEvent.Name,
            defaultListNames: defaultListNames);

        if (boardResult.IsFailure) return;

        unitOfWork.Boards.Add(boardResult.Value);
        await unitOfWork.SaveChangesAsync(ct);
    }
}