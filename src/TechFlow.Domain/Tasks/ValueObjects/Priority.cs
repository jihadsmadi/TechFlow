namespace TechFlow.Domain.Tasks.ValueObjects;

/// <summary>
/// Priority levels for a task — ordered from lowest to highest urgency.
/// Stored as string in DB via EF Core HasConversion.
/// </summary>
public static class Priority
{
    public const string Low = "Low";
    public const string Medium = "Medium";
    public const string High = "High";
    public const string Critical = "Critical";

    public static readonly IReadOnlyList<string> All =
    [
        Low,
        Medium,
        High,
        Critical
    ];

    public static bool IsValid(string priority) =>
        All.Contains(priority, StringComparer.OrdinalIgnoreCase);
}
