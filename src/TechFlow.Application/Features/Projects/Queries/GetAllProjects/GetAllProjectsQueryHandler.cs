using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Roles;


namespace TechFlow.Application.Features.Projects.Queries.GetAllProjects;

public sealed class GetAllProjectsQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<GetAllProjectsQuery, Result<List<ProjectSummaryDto>>>
{
    public async Task<Result<List<ProjectSummaryDto>>> Handle(
        GetAllProjectsQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var isAdmin = currentUser.IsInRole(SystemRoles.Admin);

        var userId = isAdmin ? (Guid?)null : currentUser.Id.Value;

        var projects = await unitOfWork.Projects.GetByCompanyAsync(
            currentUser.CompanyId,
            userId,
            query.IncludeArchived,
            ct);

        return projects.Select(p => new ProjectSummaryDto(
            Id:          p.Id,
            Name:        p.Name,
            Description: p.Description,
            Status:      p.Status,
            Color:       p.Color.Value,
            IsArchived:  p.IsArchived,
            MemberCount: p.Members.Count,
            StartDate:   p.StartDate,
            EndDate:     p.EndDate))
            .ToList();
    }
}
