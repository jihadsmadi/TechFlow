using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Roles.Mappers;

public static class RoleMapper
{
    public static RoleDto ToDto(this Role entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        IsSystemRole = entity.IsSystemRole,
        Permissions = entity.Permissions.ToDtos()
    };

    public static RoleSummaryDto ToSummaryDto(this Role entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Description = entity.Description,
        IsSystemRole = entity.IsSystemRole,
        PermissionCount = entity.Permissions.Count
    };
    public static List<RoleDto> ToDtos(this IEnumerable<Role> entities) =>
        [.. entities.Select(e => e.ToDto())];

    public static List<RoleSummaryDto> ToSummaryDtos(this IEnumerable<Role> entities) =>
        [.. entities.Select(e => e.ToSummaryDto())];

}