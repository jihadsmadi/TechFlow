using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Projects.ValueObjects;

/// <summary>
/// A validated hex color used as a visual label for a project in the UI.
/// Example: "#3b82f6"
/// </summary>
public sealed class ProjectColor
{
    public string Value { get; }

    private static readonly string[] Presets =
    [
        "#3b82f6", // blue
        "#10b981", // green
        "#f59e0b", // amber
        "#ef4444", // red
        "#8b5cf6", // purple
        "#06b6d4", // cyan
        "#f97316", // orange
        "#ec4899"  // pink
    ];

    private ProjectColor(string value) => Value = value;

    public static Result<ProjectColor> Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return ProjectColorErrors.Required;

        var normalized = raw.Trim().ToLower();

        if (!IsValidHexColor(normalized))
            return ProjectColorErrors.InvalidFormat;

        return new ProjectColor(normalized);
    }

    public static ProjectColor Default() => new(Presets[0]);

    public static IReadOnlyList<string> GetPresets() => Presets;

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is ProjectColor other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();

    private static bool IsValidHexColor(string color) =>
        color.StartsWith('#') &&
        color.Length == 7 &&
        color[1..].All(Uri.IsHexDigit);
}

public static class ProjectColorErrors
{
    public static readonly Error Required =
        Error.Validation("ProjectColor.Required", "Project color is required.");

    public static readonly Error InvalidFormat =
        Error.Validation("ProjectColor.InvalidFormat", "Color must be a valid hex value (e.g. #3b82f6).");
}