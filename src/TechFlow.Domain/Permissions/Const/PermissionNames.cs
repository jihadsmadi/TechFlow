namespace TechFlow.Domain.Permissions.Const;

/// <summary>
/// All permission names in the system.
/// Format: group.action
/// Used in policy definitions and role seeding.
/// </summary>
public static class PermissionNames
{
    // Company
    public const string CompanyRead = "company.read";
    public const string CompanyUpdate = "company.update";
    public const string CompanyCreate = "company.create";
    public const string CompanyManageSettings = "company.manage_settings";
    public const string CompanyManageFlags = "company.manage_flags";

    // Users
    public const string UsersRead = "users.read";
    public const string UsersInvite = "users.invite";
    public const string UsersDeactivate = "users.deactivate";
    public const string UsersManageRoles = "users.manage_roles";

    // Projects
    public const string ProjectsRead = "projects.read";
    public const string ProjectsCreate = "projects.create";
    public const string ProjectsUpdate = "projects.update";
    public const string ProjectsArchive = "projects.archive";
    public const string ProjectsDelete = "projects.delete";
    public const string ProjectsManageMembers = "projects.manage_members";

    // Roles
    public const string RolesRead = "roles.read";
    public const string RolesCreate = "roles.create";
    public const string RolesUpdate = "roles.update";
    public const string RolesDelete = "roles.delete";
    public const string RolesAssignPermission = "roles.assign_permission";
    public const string RolesRevokePermission = "roles.revoke_permission";

    // Permissions
    public const string PermissionsRead = "permissions.read";
    public const string PermissionsCreate = "permissions.create";
    public const string PermissionsUpdate = "permissions.update";
    public const string PermissionsDelete = "permissions.delete";

    // Tasks
    public const string TasksRead = "tasks.read";
    public const string TasksCreate = "tasks.create";
    public const string TasksUpdate = "tasks.update";
    public const string TasksDelete = "tasks.delete";
    public const string TasksAssign = "tasks.assign";
    public const string TasksMove = "tasks.move";

    // Sprints
    public const string SprintsRead = "sprints.read";
    public const string SprintsCreate = "sprints.create";
    public const string SprintsUpdate = "sprints.update";
    public const string SprintsDelete = "sprints.delete";
    public const string SprintsManage = "sprints.manage"; 

    public static readonly IReadOnlyList<string> All =
    [
        CompanyRead, CompanyUpdate, CompanyCreate, CompanyManageSettings, CompanyManageFlags,
        UsersRead, UsersInvite, UsersDeactivate, UsersManageRoles,
        ProjectsRead, ProjectsCreate, ProjectsUpdate, ProjectsArchive, ProjectsDelete, ProjectsManageMembers,
        RolesRead, RolesCreate, RolesUpdate, RolesDelete, RolesAssignPermission, RolesRevokePermission,
        PermissionsRead, PermissionsCreate, PermissionsUpdate, PermissionsDelete,
        TasksRead, TasksCreate, TasksUpdate, TasksDelete, TasksAssign, TasksMove,
        SprintsRead, SprintsCreate, SprintsUpdate, SprintsDelete, SprintsManage
    ];
}