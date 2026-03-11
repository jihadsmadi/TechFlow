using TechFlow.Application.Features.Permissions.Dtos;

namespace TechFlow.Application.Features.Roles.Dtos;

public sealed class RoleDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystemRole { get; init; }
    public List<PermissionDto> Permissions { get; init; } = [];
}
