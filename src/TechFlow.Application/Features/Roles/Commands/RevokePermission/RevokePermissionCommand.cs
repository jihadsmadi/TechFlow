using MediatR;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.RevokePermission;

public sealed record RevokePermissionCommand(
    Guid RoleId,
    Guid PermissionId
) : IRequest<Result<RoleDto>>;