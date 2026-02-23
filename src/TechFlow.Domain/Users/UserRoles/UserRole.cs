using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users.UserRoles;

/// <summary>
/// Assigns a Role to a User, optionally scoped to a specific project.
/// 
/// ProjectId = NULL  → role applies company-wide (e.g. Sarah is Admin everywhere)
/// ProjectId = value → role applies to that project only (e.g. Maria is PM on Mobile App)
/// </summary>
public sealed class UserRole : Entity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }
    public Guid? ProjectId { get; private set; }
    public Guid AssignedByUserId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }

    private UserRole() { }

    private UserRole(Guid id, Guid userId, Guid roleId, Guid? projectId, Guid assignedByUserId)
        : base(id)
    {
        UserId = userId;
        RoleId = roleId;
        ProjectId = projectId;
        AssignedByUserId = assignedByUserId;
        AssignedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<UserRole> CreateCompanyWide(Guid userId, Guid roleId, Guid assignedByUserId)
    {
        if (!IsValidId(userId))
            return UserRoleErrors.UserIdRequired;

        if (!IsValidId(roleId))
            return UserRoleErrors.RoleIdRequired;

        if (!IsValidId(assignedByUserId))
            return UserRoleErrors.AssignedByRequired;

        return new UserRole(Guid.NewGuid(), userId, roleId, null, assignedByUserId);
    }

    /// <summary>Creates a project-scoped role assignment.</summary>
    public static Result<UserRole> CreateProjectScoped(Guid userId, Guid roleId, Guid projectId, Guid assignedByUserId)
    {
        if (!IsValidId(userId))
            return UserRoleErrors.UserIdRequired;

        if (!IsValidId(roleId))
            return UserRoleErrors.RoleIdRequired;

        if (!IsValidId(projectId))
            return UserRoleErrors.ProjectIdRequired;

        if (!IsValidId(assignedByUserId))
            return UserRoleErrors.AssignedByRequired;

        return new UserRole(Guid.NewGuid(), userId, roleId, projectId, assignedByUserId);
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    public bool IsCompanyWide => ProjectId is null;
    public bool IsProjectScoped => ProjectId is not null;

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}