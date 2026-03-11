using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserCompanyRoles;

public static class UserCompanyRoleErrors
{
    public static readonly Error UserIdRequired =
        Error.Validation("UserCompanyRoleErrors.UserIdRequired", "User ID is required.");

    public static readonly Error RoleIdRequired =
        Error.Validation("UserCompanyRoleErrors.RoleIdRequired", "Role ID is required.");

    public static readonly Error AssignedByRequired =
        Error.Validation("UserCompanyRoleErrors.AssignedByRequired", "Assigned by user ID is required.");

    public static readonly Error AlreadyAssigned =
        Error.Conflict("UserCompanyRoleErrors.AlreadyAssigned", "This role is already assigned to the user in this context.");

    public static readonly Error NotFound =
        Error.NotFound("UserCompanyRoleErrors.NotFound", "Role assignment was not found.");
}