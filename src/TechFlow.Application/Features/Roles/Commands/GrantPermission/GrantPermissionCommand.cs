using MediatR;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.GrantPermission;

public sealed record GrantPermissionCommand(
    Guid RoleId,
    Guid PermissionId
) : IRequest<Result<RoleDto>>;