using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Roles.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.GrantPermission;

public sealed record GrantPermissionCommand(
    Guid RoleId,
    Guid PermissionId
) : IRequest<Result<RoleDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Roles.Tag];
}