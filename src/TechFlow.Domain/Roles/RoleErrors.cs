using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Roles;

public static class RoleErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Role.NameRequired", "Role name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Role.NameTooLong", "Role name cannot exceed 50 characters.");

    public static readonly Error NotFound =
        Error.NotFound("Role.NotFound", "Role was not found.");

    public static readonly Error AlreadyExists =
        Error.Conflict("Role.AlreadyExists", "A role with this name already exists.");

    public static readonly Error CannotModifySystemRole =
        Error.Forbidden("Role.CannotModifySystemRole", "System roles cannot be modified.");

    public static readonly Error CannotDeleteSystemRole =
        Error.Forbidden("Role.CannotDeleteSystemRole", "System roles cannot be deleted.");

    public static Error PermissionAlreadyGranted(string permissionName) =>
        Error.Conflict("Role.PermissionAlreadyGranted", $"Permission '{permissionName}' is already granted to this role.");

    public static Error PermissionNotGranted(string permissionName) =>
        Error.NotFound("Role.PermissionNotGranted", $"Permission '{permissionName}' is not granted to this role.");
}