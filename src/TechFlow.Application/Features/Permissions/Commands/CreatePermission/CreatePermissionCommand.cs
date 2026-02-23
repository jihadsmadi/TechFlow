using MediatR;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Commands.CreatePermission;

public sealed record CreatePermissionCommand(
    string Name,
    string Group,
    string Description
) : IRequest<Result<PermissionDto>>;