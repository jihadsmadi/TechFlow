using System.Text.Json;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Projects.ProjectSettings;

/// <summary>
/// Owned entity — part of the Project aggregate.
/// Mapped to PROJECT_SETTINGS table via EF Core owned entity configuration.
/// Created automatically with defaults when a Project is created.
/// </summary>
public sealed class ProjectSetting
{
    // Board defaults — used when auto-creating lists on board creation
    public string DefaultListNames { get; private set; } = """["To Do","In Progress","Done"]""";
    public string DefaultTaskType { get; private set; } = "Feature";
    public string DefaultPriority { get; private set; } = "Medium";

    // Workflow rules
    public bool AutoAssignCreator { get; private set; } = false;
    public bool RequireEstimate { get; private set; } = false;
    public bool AllowSubtasks { get; private set; } = true;

    private static readonly string[] ValidTaskTypes =
        ["Bug", "Feature", "Research", "Chore"];

    private static readonly string[] ValidPriorities =
        ["Low", "Medium", "High", "Critical"];

    private ProjectSetting() { }

    internal static ProjectSetting CreateDefault() => new();

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Update(
        List<string> defaultListNames,
        string defaultTaskType,
        string defaultPriority,
        bool autoAssignCreator,
        bool requireEstimate,
        bool allowSubtasks)
    {
        if (!IsValidListNames(defaultListNames))
            return ProjectSettingsErrors.InvalidListNames;

        if (!IsValidTaskType(defaultTaskType))
            return ProjectSettingsErrors.InvalidTaskType(defaultTaskType);

        if (!IsValidPriority(defaultPriority))
            return ProjectSettingsErrors.InvalidPriority(defaultPriority);

        DefaultListNames = JsonSerializer.Serialize(defaultListNames);
        DefaultTaskType = defaultTaskType;
        DefaultPriority = defaultPriority;
        AutoAssignCreator = autoAssignCreator;
        RequireEstimate = requireEstimate;
        AllowSubtasks = allowSubtasks;

        return Result.Updated;
    }

    /// <summary>
    /// Returns the default list names as a typed list.
    /// Used when auto-creating board lists on project creation.
    /// </summary>
    public List<string> GetDefaultListNames() =>
        JsonSerializer.Deserialize<List<string>>(DefaultListNames) ?? ["To Do", "In Progress", "Done"];

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidListNames(List<string> names) =>
        names.Count >= 1 &&
        names.Count <= 10 &&
        names.All(n => !string.IsNullOrWhiteSpace(n));

    private static bool IsValidTaskType(string type) => ValidTaskTypes.Contains(type);
    private static bool IsValidPriority(string priority) => ValidPriorities.Contains(priority);
}
