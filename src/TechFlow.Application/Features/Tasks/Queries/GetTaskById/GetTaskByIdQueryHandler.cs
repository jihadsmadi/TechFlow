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

        var (completed, total) = task.GetSubtaskProgress();

        return new TaskDto(
            Id: task.Id,
            ListId: task.ListId,
            ProjectId: task.ProjectId,
            CompanyId: task.CompanyId,
            CreatedByUserId: task.CreatedByUserId,
            Title: task.Title,
            Description: task.Description,
            Priority: task.Priority,
            Type: task.Type,
            DisplayOrder: task.DisplayOrder,
            DueDate: task.DueDate,
            EstimatedMinutes: task.EstimatedMinutes,
            ActualMinutes: task.ActualMinutes,
            IsCompleted: task.IsCompleted,
            CompletedAt: task.CompletedAt,
            Subtasks: task.Subtasks
                .Select(s => new SubtaskDto(
                    Id: s.Id,
                    TaskId: s.TaskId,
                    Title: s.Title,
                    IsCompleted: s.IsCompleted,
                    CreatedAt: s.CreatedAt))
                .ToList(),
            SubtasksTotal: total,
            SubtasksCompleted: completed,
            CreatedAt: task.CreatedAtUtc,
            UpdatedAt: task.LastModifiedUtc);
    }
}