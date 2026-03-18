using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Sprints.Queries.GetBacklog;

public sealed class GetBacklogQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetBacklogQuery, Result<IReadOnlyList<TaskSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<TaskSummaryDto>>> Handle(
        GetBacklogQuery query,
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

        // backlog = tasks in this project that are NOT in any active or planned sprint
        var tasks = await unitOfWork.Tasks.GetBacklogTasksAsync(query.ProjectId, ct);

        return tasks
            .Select(t => new TaskSummaryDto(
                Id: t.Id,
                ListId: t.ListId,
                Title: t.Title,
                Priority: t.Priority,
                Type: t.Type,
                DisplayOrder: t.DisplayOrder,
                DueDate: t.DueDate,
                IsCompleted: t.IsCompleted,
                SubtasksTotal: t.Subtasks.Count,
                SubtasksCompleted: t.Subtasks.Count(s => s.IsCompleted)))
            .ToList()
            .AsReadOnly();
    }
}
