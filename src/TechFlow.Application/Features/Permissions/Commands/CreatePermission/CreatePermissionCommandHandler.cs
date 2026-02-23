using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;

namespace TechFlow.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreatePermissionCommandHandler> logger)
    : IRequestHandler<CreatePermissionCommand, Result<PermissionDto>>
{
    public async Task<Result<PermissionDto>> Handle(
        CreatePermissionCommand command,
        CancellationToken ct)
    {
        var exists = await unitOfWork.Permissions.ExistsByNameAsync(command.Name, ct);
        if (exists)
        {
            logger.LogWarning("Permission name already exists: {Name}", command.Name);
            return PermissionErrors.AlreadyExists;
        }

        var result = Permission.Create(command.Name, command.Group, command.Description);
        if (result.IsFailure)
        {
            logger.LogWarning("Permission creation failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Permissions.Add(result.Value);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Permission created: {Name}", result.Value.Name);

        return result.Value.ToDto();
    }
}