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

    public static readonly IReadOnlyList<string> All =
    [
        Admin,
        ProjectManager,
        Developer,
        Intern
    ];
}