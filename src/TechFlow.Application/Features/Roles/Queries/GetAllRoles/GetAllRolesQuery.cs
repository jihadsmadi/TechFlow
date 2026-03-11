using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.Dtos;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<Result<List<RoleSummaryDto>>>, ICachedQuery
{
    public string CacheKey => CacheKeys.Roles.All;
    public string[] Tags => [CacheKeys.Roles.Tag];
    public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Roles.ExpirationHours);
}