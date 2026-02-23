using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Queries.GetPermissionById;

public sealed record GetPermissionByIdQuery(Guid Id)
    : IRequest<Result<PermissionDto>>, ICachedQuery
{
    public string CacheKey => $"permissions:{Id}";
    public string[] Tags => ["permissions"];
    public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.PermissionExpirTimeSpan);
}