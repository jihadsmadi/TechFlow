using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Application.Features.Roles.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateRoleCommandHandler> logger)
    : IRequestHandler<UpdateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(
        UpdateRoleCommand command,
        CancellationToken ct)
    {
        var role = await unitOfWork.Roles.GetByIdAsync(command.Id, ct);
        if (role is null)
        {
            logger.LogWarning("Role not found: {Id}", command.Id);
            return RoleErrors.NotFound;
        }

        var nameChanged = !role.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase);
        if (nameChanged)
        {
            var exists = await unitOfWork.Roles.ExistsByNameAsync(command.Name, ct);
            if (exists)
            {
                logger.LogWarning("Role name already exists: {Name}", command.Name);
                return RoleErrors.AlreadyExists;
            }
        }

        var result = role.Update(command.Name, command.Description);
        if (result.IsFailure)
        {
            logger.LogWarning("Role update failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Roles.Update(role);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Role updated: {Id}", role.Id);

        return role.ToDto();
    }
}