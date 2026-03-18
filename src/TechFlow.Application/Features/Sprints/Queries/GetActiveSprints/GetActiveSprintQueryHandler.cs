using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Application.Features.Sprints.Mappers;
using TechFlow.Application.Features.Sprints.Queries.GetSprintById;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Sprints.Queries.GetActiveSprints;

public sealed class GetActiveSprintQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetActiveSprintQuery, Result<SprintDto?>>
{
    public async Task<Result<SprintDto?>> Handle(GetActiveSprintQuery query, CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var project = await unitOfWork.Projects.GetByIdWithMembersAsync(query.ProjectId, ct);
        if (project is null)
            return ProjectErrors.NotFound;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);
        if (!accessService.CanAccess(project, currentUser.Id.Value, isAdmin))
            return ProjectErrors.AccessDenied;

        var sprint = await unitOfWork.Sprints.GetActiveByProjectIdAsync(query.ProjectId, ct);

        if (sprint is null)
            return (SprintDto?)null;

        var taskIds = sprint.Items.Select(i => i.TaskId).ToList();
        var tasks = await unitOfWork.Tasks.GetByIdsAsync(taskIds, ct);

        var taskDtos = tasks
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
                SubtasksCompleted: t.Subtasks.Count(s => s.IsCompleted))).ToList();

        return sprint.ToDto(taskDtos);
    }
}
