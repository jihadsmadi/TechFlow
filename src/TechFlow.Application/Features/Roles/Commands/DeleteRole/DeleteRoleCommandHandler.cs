using MediatR;
using Microsoft.Extensions.Logging;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Commands.DeleteRole;

public sealed class DeleteRoleCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteRoleCommandHandler> logger)
    : IRequestHandler<DeleteRoleCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(
        DeleteRoleCommand command,
        CancellationToken ct)
    {
        var role = await unitOfWork.Roles.GetByIdAsync(command.Id, ct);
        if (role is null)
        {
            logger.LogWarning("Role not found: {Id}", command.Id);
            return RoleErrors.NotFound;
        }

        var result = role.Delete();
        if (result.IsFailure)
        {
            logger.LogWarning("Role delete failed: {Errors}", result.Errors);
            return result.Errors;
        }

        unitOfWork.Roles.Remove(role);
        await unitOfWork.SaveChangesAsync(ct);

        logger.LogInformation("Role deleted: {Id}", command.Id);

        return Result.Deleted;
    }
}