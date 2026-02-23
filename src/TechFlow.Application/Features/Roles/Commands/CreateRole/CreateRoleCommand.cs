using MediatR;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Roles.Commands.CreateRole;

public sealed record CreateRoleCommand(
    string Name,
    string Description
) : IRequest<Result<RoleDto>>;