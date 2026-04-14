using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Tasks.Commands.UpdateTask;

public sealed class UpdateTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<UpdateTaskCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(UpdateTaskCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        if (project.Settings.RequireEstimate && !command.EstimatedMinutes.HasValue)
            return TaskErrors.EstimateRequired;

        var task = await unitOfWork.Tasks.GetByIdAsync(command.TaskId, ct);
        if (task is null)
            return TaskErrors.NotFound;
        if (task.ProjectId != command.ProjectId)
            return TaskErrors.NotFound;

        var result = task.Update(
            title: command.Title,
            description: command.Description,
            priority: command.Priority,
            type: command.Type,
            dueDate: command.DueDate,
            estimatedMinutes: command.EstimatedMinutes);

        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
