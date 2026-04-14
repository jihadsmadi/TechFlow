using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Sprints;

namespace TechFlow.Application.Features.Sprints.Commands.EndSprint;

public sealed class EndSprintCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<EndSprintCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(EndSprintCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var sprint = await unitOfWork.Sprints.GetByIdWithItemsAsync(command.SprintId, ct);
        if (sprint is null)
            return SprintErrors.NotFound;

        if (sprint.ProjectId != command.ProjectId)
            return SprintErrors.NotFound;
        // End() fires SprintEndedEvent — SprintEndedEventHandler handles incomplete tasks
        var result = sprint.End(command.IncompleteTasksAction);
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
