using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.Dtos;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(Guid Id)
    : IRequest<Result<RoleDto>>, ICachedQuery
{
    public string CacheKey => CacheKeys.Roles.ById(Id);
    public string[] Tags => [CacheKeys.Roles.Tag];
    public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Roles.ExpirationHours);
}