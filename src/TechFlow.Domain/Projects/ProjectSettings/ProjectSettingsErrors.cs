using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Projects.ProjectSettings;

public static class ProjectSettingsErrors
{
    public static readonly Error InvalidListNames =
        Error.Validation("ProjectSettings.InvalidListNames",
            "Must provide between 1 and 10 non-empty list names.");

    public static Error InvalidTaskType(string type) =>
        Error.Validation("ProjectSettings.InvalidTaskType",
            $"'{type}' is not valid. Valid types: Bug, Feature, Research, Chore.");

    public static Error InvalidPriority(string priority) =>
        Error.Validation("ProjectSettings.InvalidPriority",
            $"'{priority}' is not valid. Valid priorities: Low, Medium, High, Critical.");
}