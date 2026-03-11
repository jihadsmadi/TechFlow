using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserCompanyRoles;

public sealed class UserCompanyRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid AssignedByUserId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }

    private UserCompanyRole() { }

    private UserCompanyRole(Guid id, Guid userId, Guid roleId, Guid assignedByUserId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        AssignedByUserId = assignedByUserId;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory 

    public static Result<UserCompanyRole> Create(Guid userId, Guid roleId, Guid assignedByUserId)
    {
        if (!IsValidId(userId))
            return UserCompanyRoleErrors.UserIdRequired;

        if (!IsValidId(roleId))
            return UserCompanyRoleErrors.RoleIdRequired;

        if (!IsValidId(assignedByUserId))
            return UserCompanyRoleErrors.AssignedByRequired;

        return new UserCompanyRole(Guid.NewGuid(), userId, roleId, assignedByUserId);
    }

   

    // ── Private Validation 

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}