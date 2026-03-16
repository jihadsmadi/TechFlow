namespace TechFlow.Domain.Tasks.ValueObjects;

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
