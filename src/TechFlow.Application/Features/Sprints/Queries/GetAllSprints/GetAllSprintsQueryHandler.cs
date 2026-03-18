using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Application.Features.Sprints.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Sprints.Queries.GetAllSprints;

public sealed class GetAllSprintsQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetAllSprintsQuery, Result<IReadOnlyList<SprintSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<SprintSummaryDto>>> Handle(
        GetAllSprintsQuery query,
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

        var sprints = await unitOfWork.Sprints.GetByProjectIdAsync(query.ProjectId, ct);

        // for summary we need task counts — batch load all sprint task ids
        var allSprintIds = sprints.Select(s => s.Id).ToList();
        var taskCountsPerSprint = await unitOfWork.Tasks
            .GetCountsBySprintIdsAsync(allSprintIds, ct);

        return sprints.ToSummaryDtos(taskCountsPerSprint);
    }
}
