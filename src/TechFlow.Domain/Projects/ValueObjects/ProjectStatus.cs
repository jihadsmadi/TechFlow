namespace TechFlow.Domain.Projects.ValueObjects;

/// <summary>
/// Represents the lifecycle status of a project.
/// </summary>
public static class ProjectStatus
{
    public const string Active = "Active";
    public const string OnHold = "OnHold";
    public const string Completed = "Completed";
    public const string Archived = "Archived";

    public static readonly IReadOnlyList<string> All =
    [
        Active,
        OnHold,
        Completed,
        Archived
    ];

    public static bool IsValid(string status) =>
        All.Contains(status, StringComparer.OrdinalIgnoreCase);
}
