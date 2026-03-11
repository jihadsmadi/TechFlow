namespace TechFlow.Domain.Permissions.Const;

public static class PermissionGroups
{
    public const string Company = "Company";
    public const string Projects = "Projects";
    public const string Permissions = "Permissions";
    public const string Roles = "Roles";
    public const string Tasks = "Tasks";
    public const string Users = "Users";
    public const string Reports = "Reports";

    public static readonly IReadOnlyList<string> All =
    [
        Company,
        Projects,
        Permissions,
        Roles,
        Tasks,
        Users,
        Reports
    ];
}
