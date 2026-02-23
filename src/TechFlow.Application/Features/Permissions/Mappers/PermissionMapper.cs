using TechFlow.Application.Features.Permissions.DTOs;
using TechFlow.Domain.Permissions;

namespace TechFlow.Application.Features.Permissions.Mappers;

public static class PermissionMapper
{
    public static PermissionDto ToDto(this Permission entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Group = entity.Group,
        Description = entity.Description
    };

    public static List<PermissionDto> ToDtos(this IEnumerable<Permission> entities) =>
        [.. entities.Select(e => e.ToDto())];
}