namespace TechFlow.Domain.Tasks.ValueObjects;

/// <summary>
/// Classifies what kind of work a task represents.
/// Drives which custom fields are shown in the UI.
/// </summary>
public static class TaskType
{
    public const string Bug = "Bug";
    public const string Feature = "Feature";
    public const string Research = "Research";
    public const string Chore = "Chore";

    public static readonly IReadOnlyList<string> All =
    [
        Bug,
        Feature,
        Research,
        Chore
    ];

    public static bool IsValid(string type) =>
        All.Contains(type, StringComparer.OrdinalIgnoreCase);
}