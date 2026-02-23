using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(Guid Id)
    : IRequest<Result<RoleDto>>, ICachedQuery
{
    public string CacheKey => $"roles:{Id}";
    public string[] Tags => ["roles"];
    public TimeSpan Expiration => TimeSpan.FromHours(TechFlowConstants.Cashe.RoleExpirTimeSpan);
}