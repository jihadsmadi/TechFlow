using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Services;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Projects.Queries.GetProjectMembers;

public sealed class GetProjectMembersQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    ProjectAccessService accessService)
    : IRequestHandler<GetProjectMembersQuery, Result<List<ProjectMemberDto>>>
{
    public async Task<Result<List<ProjectMemberDto>>> Handle(
        GetProjectMembersQuery query,
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

        return project.Members
            .Select(m => new ProjectMemberDto(
                Id:            m.Id,
                UserId:        m.UserId,
                ProjectId:     m.ProjectId,
                AddedByUserId: m.AddedByUserId,
                AddedAt:       m.AddedAt))
            .ToList();
    }
}
