using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;


namespace TechFlow.Application.Features.Tasks.Commands.AddSubtask;

public sealed class AddSubtaskCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<AddSubtaskCommand, Result<SubtaskDto>>
{
    public async Task<Result<SubtaskDto>> Handle(AddSubtaskCommand command, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(command.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        if (!project.Settings.AllowSubtasks)
            return TaskErrors.SubtasksNotAllowed;
        var task = await unitOfWork.Tasks.GetByIdWithSubtasksAsync(command.TaskId, ct);

        if (task is null)
            return TaskErrors.NotFound;

        var result = task.AddSubtask(currentUser.Id.Value, command.Title);
        if (result.IsFailure)
            return result.TopError;

        unitOfWork.Tasks.MarkSubtaskAsAdded(result.Value);

        await unitOfWork.SaveChangesAsync(ct);

        var subtask = result.Value;
        return new SubtaskDto(
            Id: subtask.Id,
            TaskId: subtask.TaskId,
            Title: subtask.Title,
            IsCompleted: subtask.IsCompleted,
            CreatedAt: subtask.CreatedAt);
    }
}
