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

    // Tasks
    public const string TasksRead = "tasks.read";
    public const string TasksCreate = "tasks.create";
    public const string TasksUpdate = "tasks.update";
    public const string TasksDelete = "tasks.delete";
    public const string TasksAssign = "tasks.assign";
    public const string TasksMove = "tasks.move";
}