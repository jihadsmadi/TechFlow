namespace TechFlow.Domain.Sprints.ValueObjects;

public static class SprintStatus
{
    public const string Planned   = "Planned";
    public const string Active    = "Active";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly IReadOnlyList<string> All =
    [
        Planned,
        Active,
        Completed,
        Cancelled
    ];

    public static bool IsValid(string status) =>
        All.Contains(status, StringComparer.OrdinalIgnoreCase);
}
