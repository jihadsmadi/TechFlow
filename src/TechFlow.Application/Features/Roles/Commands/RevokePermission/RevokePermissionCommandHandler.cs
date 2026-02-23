using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Application.Features.Roles.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Commands.RevokePermission;

public sealed class RevokePermissionCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<RevokePermissionCommandHandler> logger)
    : IRequestHandler<RevokePermissionCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(
        RevokePermissionCommand command,
        CancellationToken ct)
    {
        var role = await unitOfWork.Roles.GetWithPermissionsAsync(command.RoleId, ct);
        if (role is null)
        {
            logger.LogWarning("Role not found: {RoleId}", command.RoleId);
            return RoleErrors.NotFound;
        }

        var permission = await unitOfWork.Permissions.GetByIdAsync(command.PermissionId, ct);
        if (permission is null)
        {
            logger.LogWarning("Permission not found: {PermissionId}", command.PermissionId);
            return PermissionErrors.NotFound;
        }

        var result = role.RevokePermission(permission);
        if (result.IsFailure)
        {
            logger.LogWarning("Revoke permission failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Roles.Update(role);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation(
            "Permission {PermissionName} revoked from role {RoleName}",
            permission.Name, role.Name);

        return role.ToDto();
    }
}