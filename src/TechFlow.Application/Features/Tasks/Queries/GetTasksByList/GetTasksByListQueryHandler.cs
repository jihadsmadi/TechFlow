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

namespace TechFlow.Application.Features.Tasks.Queries.GetTasksByList;

public sealed class GetTasksByListQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetTasksByListQuery, Result<IReadOnlyList<TaskSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<TaskSummaryDto>>> Handle(
        GetTasksByListQuery query,
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

        var tasks = await unitOfWork.Tasks.GetByListIdAsync(query.ListId, ct);

        return tasks.ToSummaryDtos();
    }
}
