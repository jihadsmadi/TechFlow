using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Queries.GetPermissionById;

public sealed record GetPermissionByIdQuery(Guid Id)
    : IRequest<Result<PermissionDto>>, ICachedQuery
{
    public string CacheKey => CacheKeys.Permissions.ById(Id);
    public string[] Tags => [CacheKeys.Permissions.Tag];
    public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Permissions.ExpirationHours);
}