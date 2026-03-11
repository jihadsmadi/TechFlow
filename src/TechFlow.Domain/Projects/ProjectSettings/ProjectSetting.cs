using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Tasks.ValueObjects;

namespace TechFlow.Domain.Projects.ProjectSettings;

public sealed class ProjectSetting
{
    private static readonly string[] DefaultLists = ["Backlog", "To Do", "In Progress", "Done"];

    public string DefaultListNames { get; private set; } = string.Join(",", DefaultLists);
    public string DefaultTaskType { get; private set; } = TaskType.Feature;
    public string DefaultPriority { get; private set; } = Priority.Medium;
    public bool AutoAssignCreator { get; private set; } = false;
    public bool RequireEstimate { get; private set; } = false;
    public bool AllowSubtasks { get; private set; } = true;
    public bool SprintLockOnStart { get; private set; } = false;
    public int SprintDurationDays { get; private set; } = 14;
    public string IncompleteTasksAction { get; private set; } = IncompleteTasksActionType.MoveToBacklog;

    private ProjectSetting() { }

    internal static ProjectSetting Default() => new();

    public Result<Updated> UpdateSprintSettings(
        bool? sprintLockOnStart = null,
        int? sprintDurationDays = null,
        string? incompleteTasksAction = null)
    {
        if (sprintDurationDays.HasValue && sprintDurationDays.Value < 1)
            return ProjectSettingErrors.InvalidSprintDuration;

        if (incompleteTasksAction is not null &&
            !IncompleteTasksActionType.IsValid(incompleteTasksAction))
            return ProjectSettingErrors.InvalidIncompleteTasksAction;

        if (sprintLockOnStart.HasValue)
            SprintLockOnStart = sprintLockOnStart.Value;

        if (sprintDurationDays.HasValue)
            SprintDurationDays = sprintDurationDays.Value;

        if (incompleteTasksAction is not null)
            IncompleteTasksAction = incompleteTasksAction;

        return Result.Updated;
    }

    public Result<Updated> Update(
    List<string>? defaultListNames = null,
    string? defaultTaskType = null,
    string? defaultPriority = null,
    bool? autoAssignCreator = null,
    bool? requireEstimate = null,
    bool? allowSubtasks = null)
    {
        if (defaultTaskType is not null && !TaskType.IsValid(defaultTaskType))
            return ProjectSettingErrors.InvalidTaskType;

        if (defaultPriority is not null && !Priority.IsValid(defaultPriority))
            return ProjectSettingErrors.InvalidPriority;

        if (defaultListNames is not null && defaultListNames.Count > 0)
            DefaultListNames = string.Join(",", defaultListNames);

        if (defaultTaskType is not null)
            DefaultTaskType = defaultTaskType;

        if (defaultPriority is not null)
            DefaultPriority = defaultPriority;

        if (autoAssignCreator.HasValue)
            AutoAssignCreator = autoAssignCreator.Value;

        if (requireEstimate.HasValue)
            RequireEstimate = requireEstimate.Value;

        if (allowSubtasks.HasValue)
            AllowSubtasks = allowSubtasks.Value;

        return Result.Updated;
    }
    public IEnumerable<string> GetDefaultListNames()
        => DefaultListNames.Split(',', StringSplitOptions.RemoveEmptyEntries);
}

public static class IncompleteTasksActionType
{
    public const string MoveToBacklog = "MoveToBacklog";
    public const string MoveToNextSprint = "MoveToNextSprint";
    public const string LeaveInPlace = "LeaveInPlace";

    public static readonly IReadOnlyList<string> All =
        [MoveToBacklog, MoveToNextSprint, LeaveInPlace];

    public static bool IsValid(string value) =>
        All.Contains(value, StringComparer.OrdinalIgnoreCase);
}

public static class ProjectSettingErrors
{
    public static readonly Error InvalidSprintDuration =
        Error.Validation("ProjectSetting.InvalidSprintDuration",
            "Sprint duration must be at least 1 day.");

    public static readonly Error InvalidIncompleteTasksAction =
        Error.Validation("ProjectSetting.InvalidIncompleteTasksAction",
            $"Invalid incomplete tasks action. Valid values: {string.Join(", ", IncompleteTasksActionType.All)}");
    public static readonly Error InvalidTaskType =
    Error.Validation("ProjectSetting.InvalidTaskType", "Invalid task type.");

    public static readonly Error InvalidPriority =
        Error.Validation("ProjectSetting.InvalidPriority", "Invalid priority.");
}