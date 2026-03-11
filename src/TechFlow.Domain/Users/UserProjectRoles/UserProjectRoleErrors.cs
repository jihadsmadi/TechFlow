using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserProjectRoles;

public static class UserProjectRoleErrors
{
    public static readonly Error UserIdRequired =
        Error.Validation("UserProjectRole.UserIdRequired", "User ID is required.");

    public static readonly Error RoleIdRequired =
        Error.Validation("UserProjectRole.RoleIdRequired", "Role ID is required.");

    public static readonly Error ProjectIdRequired =
        Error.Validation("UserProjectRole.ProjectIdRequired", "Project ID is required for a project-scoped role.");

    public static readonly Error AssignedByRequired =
        Error.Validation("UserProjectRole.AssignedByRequired", "Assigned by user ID is required.");

    public static readonly Error AlreadyAssigned =
        Error.Conflict("UserProjectRole.AlreadyAssigned", "This role is already assigned to the user in this context.");

    public static readonly Error NotFound =
        Error.NotFound("UserProjectRole.NotFound", "Role assignment was not found.");
}