using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Sprints;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Sprints.Commands.AddTaskToSprint;

public sealed class AddTaskToSprintCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<AddTaskToSprintCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(AddTaskToSprintCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var sprint = await unitOfWork.Sprints.GetByIdWithItemsAsync(command.SprintId, ct);
        if (sprint is null)
            return SprintErrors.NotFound;

        // verify task exists and belongs to same project
        var task = await unitOfWork.Tasks.GetByIdAsync(command.TaskId, ct);
        if (task is null)
            return TaskErrors.NotFound;

        if (task.ProjectId != command.ProjectId)
            return TaskErrors.NotFound;

        var result = sprint.AddTask(command.TaskId);
        if (result.IsFailure)
            return result.TopError;

        unitOfWork.Sprints.MarkSprintItemAsAdded(result.Value);

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
