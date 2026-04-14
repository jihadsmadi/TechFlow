using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Tasks.Commands.MoveTask;

public sealed class MoveTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<MoveTaskCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(MoveTaskCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var task = await unitOfWork.Tasks.GetByIdAsync(command.TaskId, ct);
        if (task is null)
            return TaskErrors.NotFound;

        if (task.ProjectId != command.ProjectId) 
            return TaskErrors.NotFound;

        var isListInProject = await unitOfWork.Boards.ListBelongsToProjectAsync(command.NewListId, command.ProjectId, ct);
        if (!isListInProject)
            return TaskErrors.ListNotBelongToProject;

        var result = task.MoveToList(command.NewListId, command.PrevDisplayOrder, command.NextDisplayOrder);
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}