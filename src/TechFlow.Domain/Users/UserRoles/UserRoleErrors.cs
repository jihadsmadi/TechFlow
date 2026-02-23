using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserRoles;

public static class UserRoleErrors
{
    public static readonly Error UserIdRequired =
        Error.Validation("UserRole.UserIdRequired", "User ID is required.");

    public static readonly Error RoleIdRequired =
        Error.Validation("UserRole.RoleIdRequired", "Role ID is required.");

    public static readonly Error ProjectIdRequired =
        Error.Validation("UserRole.ProjectIdRequired", "Project ID is required for a project-scoped role.");

    public static readonly Error AssignedByRequired =
        Error.Validation("UserRole.AssignedByRequired", "Assigned by user ID is required.");

    public static readonly Error AlreadyAssigned =
        Error.Conflict("UserRole.AlreadyAssigned", "This role is already assigned to the user in this context.");

    public static readonly Error NotFound =
        Error.NotFound("UserRole.NotFound", "Role assignment was not found.");
}