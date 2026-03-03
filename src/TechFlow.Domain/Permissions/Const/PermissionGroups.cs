namespace TechFlow.Domain.Permissions.Const;

public static class PermissionGroups
{
    public const string Company = "Company";
    public const string Projects = "Projects";
    public const string Tasks = "Tasks";
    public const string Users = "Users";
    public const string Reports = "Reports";

    public static readonly IReadOnlyList<string> All =
    [
        Company,
        Projects,
        Tasks,
        Users,
        Reports
    ];
}
