using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Tasks.Commands.AssignTask;

public sealed class AssignTaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<AssignTaskCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(AssignTaskCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        // verify the user being assigned is a member of the project
        if (!isAdmin && !project.IsMember(command.UserId))
            return ProjectErrors.MemberNotFound;

        var task = await unitOfWork.Tasks.GetByIdWithAssignmentsAsync(command.TaskId, ct);
        if (task is null)
            return TaskErrors.NotFound;

        var result = task.AssignUser(command.UserId, currentUser.Id.Value);
        if (result.IsFailure)
            return result.TopError;

        unitOfWork.Tasks.MarkTaskAssignmentAsAdded(result.Value);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
