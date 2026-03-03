namespace TechFlow.Domain.Roles;

/// <summary>
/// Names of protected system roles that are seeded on startup
/// and cannot be deleted or renamed.
/// </summary>
public static class SystemRoles
{
    public const string Admin = "Admin";
    public const string ProjectManager = "ProjectManager";
    public const string Developer = "Developer";
    public const string Intern = "Intern";

    public const string Description_Admin = "Full access to all features and settings.";
    public const string Description_ProjectManager = "Can manage projects, assign tasks, and view reports.";
    public const string Description_Developer = "Can view and update assigned tasks, and comment on tasks.";
    public const string Description_Intern = "Limited access to view projects and tasks, but cannot make changes.";

    public static readonly IReadOnlyList<(string name,string description)> All =
    [
        (Admin,Description_Admin),
        (ProjectManager,Description_ProjectManager),
        (Developer,Description_Developer),
        (Intern, Description_Intern)
    ];
}