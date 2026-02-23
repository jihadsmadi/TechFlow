using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Application.Features.Roles.Mappers;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateRoleCommandHandler> logger)
    : IRequestHandler<CreateRoleCommand, Result<RoleDto>>
{
    public async Task<Result<RoleDto>> Handle(
        CreateRoleCommand command,
        CancellationToken ct)
    {
        var exists = await unitOfWork.Roles.ExistsByNameAsync(command.Name, ct);
        if (exists)
        {
            logger.LogWarning("Role name already exists: {Name}", command.Name);
            return RoleErrors.AlreadyExists;
        }

        var result = Role.Create(command.Name, command.Description);
        if (result.IsFailure)
        {
            logger.LogWarning("Role creation failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Roles.Add(result.Value);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Role created: {Name}", result.Value.Name);

        return result.Value.ToDto();
    }
}