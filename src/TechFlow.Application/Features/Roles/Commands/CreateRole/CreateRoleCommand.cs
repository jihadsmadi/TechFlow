using MediatR;
using TechFlow.Application.Common.Behaviours;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<Result<RoleDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Roles.Tag];
}