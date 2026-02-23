using MediatR;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.UpdateRole;

public sealed record UpdateRoleCommand(
    Guid Id,
    string Name,
    string Description
) : IRequest<Result<RoleDto>>;