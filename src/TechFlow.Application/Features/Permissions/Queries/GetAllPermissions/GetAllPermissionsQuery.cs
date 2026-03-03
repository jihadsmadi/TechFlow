using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery(string? Group = null)
    : IRequest<Result<List<PermissionDto>>>, ICachedQuery
{
    public string CacheKey => Group is null ? CacheKeys.Permissions.All : CacheKeys.Permissions.ByGroup(Group);
    public string[] Tags => [CacheKeys.Permissions.Tag];
    public TimeSpan Expiration => TimeSpan.FromHours(CacheKeys.Permissions.ExpirationHours);
}