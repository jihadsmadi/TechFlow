using MediatR;
using TechFlow.Application.Common.Constants;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Permissions.Commands.UpdatePermissionDescription;

public sealed record UpdatePermissionDescriptionCommand(
    Guid Id,
    string Description
) : IRequest<Result<PermissionDto>>, ICacheInvalidator
{
    public string[] Tags => [CacheKeys.Permissions.Tag];
};