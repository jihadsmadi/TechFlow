using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Application.Features.Tasks.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Tasks;

namespace TechFlow.Application.Features.Tasks.Queries.GetTaskById;

public sealed class GetTaskByIdQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetTaskByIdQuery, Result<TaskDto>>
{
    public async Task<Result<TaskDto>> Handle(
        GetTaskByIdQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(query.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var task = await unitOfWork.Tasks.GetByIdWithSubtasksAsync(query.TaskId, ct);
        if (task is null)
            return TaskErrors.NotFound;
        if (task.ProjectId != query.ProjectId)
            return TaskErrors.NotFound;
        var (completed, total) = task.GetSubtaskProgress();

        return task.ToDto();
    }
}