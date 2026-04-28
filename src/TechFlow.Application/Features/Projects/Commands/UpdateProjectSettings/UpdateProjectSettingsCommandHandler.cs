using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;

public sealed class UpdateProjectSettingsCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<UpdateProjectSettingsCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateProjectSettingsCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanModify(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var updateResult = project.Settings.Update(
            command.DefaultListNames,
            command.DefaultTaskType,
            command.DefaultPriority,
            command.AutoAssignCreator,
            command.RequireEstimate,
            command.AllowSubtasks);

        if (updateResult.IsFailure)
            return updateResult.TopError;

        var sprintUpdateResult = project.Settings.UpdateSprintSettings(
            command.SprintLockOnStart,
            command.SprintDurationDays,
            command.IncompleteTasksAction);

        if (sprintUpdateResult.IsFailure)
            return sprintUpdateResult.TopError;

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Updated;
    }
}