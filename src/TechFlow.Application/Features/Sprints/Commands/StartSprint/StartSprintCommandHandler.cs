using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Sprints;

namespace TechFlow.Application.Features.Sprints.Commands.StartSprint;

public sealed class StartSprintCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<StartSprintCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(StartSprintCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        // enforce one active sprint at a time
        var hasActive = await unitOfWork.Sprints.HasActiveSprintAsync(command.ProjectId, ct);
        if (hasActive)
            return SprintErrors.AlreadyActive;

        var sprint = await unitOfWork.Sprints.GetByIdAsync(command.SprintId, ct);
        if (sprint is null)
            return SprintErrors.NotFound;

        if (sprint.ProjectId != command.ProjectId)
            return SprintErrors.NotFound;
        // pass SprintLockOnStart from project settings
        var result = sprint.Start(project.Settings.SprintLockOnStart);
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
