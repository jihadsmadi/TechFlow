using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Commands.CreatePermission;

public sealed record CreatePermissionCommand(
    string Name,
    string Group,
    string Description
) : IRequest<Result<PermissionDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Permissions.Tag];
};