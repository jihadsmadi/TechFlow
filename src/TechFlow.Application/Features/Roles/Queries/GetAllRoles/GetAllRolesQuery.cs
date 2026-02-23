using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<Result<List<RoleSummaryDto>>>, ICachedQuery
{
    public string CacheKey => "roles:all";
    public string[] Tags => ["roles"];
    public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.RoleExpirTimeSpan);
}