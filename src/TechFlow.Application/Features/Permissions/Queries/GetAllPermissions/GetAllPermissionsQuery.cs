using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Queries.GetAllPermissions;

public sealed record GetAllPermissionsQuery(string? Group = null)
    : IRequest<Result<List<PermissionDto>>>, ICachedQuery
{
    public string CacheKey => Group is null ? "permissions:all" : $"permissions:group:{Group}";
    public string[] Tags => ["permissions"];
    public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.PermissionExpirTimeSpan);
}