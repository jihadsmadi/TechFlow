using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;

namespace TechFlow.Application.Features.Permissions.Commands.UpdatePermissionDescription;

public sealed class UpdatePermissionDescriptionCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdatePermissionDescriptionCommandHandler> logger)
    : IRequestHandler<UpdatePermissionDescriptionCommand, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(
        UpdatePermissionDescriptionCommand command,
        CancellationToken ct)
    {
        var permission = await unitOfWork.Permissions.GetByIdAsync(command.Id, ct);
        if (permission is null)
        {
            logger.LogWarning("Permission not found: {Id}", command.Id);
            return PermissionErrors.NotFound;
        }

        var result = permission.UpdateDescription(command.Description);
        if (result.IsFailure)
        {
            logger.LogWarning("Permission update failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Permissions.Update(permission);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Permission updated: {Id}", permission.Id);

        return permission.ToDto();
    }
}