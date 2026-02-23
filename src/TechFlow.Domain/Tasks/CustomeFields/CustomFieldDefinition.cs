using System.Text.Json;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.CustomeFields;

/// <summary>
/// Defines a custom field template for a project.
/// e.g. "Bug tasks in Mobile App project have a Severity field (Dropdown)"
/// </summary>
public sealed class CustomFieldDefinition : Entity
{
    public Guid ProjectId { get; private set; }
    public string? TaskType { get; private set; }  // null = applies to all types
    public string FieldName { get; private set; } = string.Empty;
    public string FieldType { get; private set; } = string.Empty;
    public string? Options { get; private set; }  // JSON array — Dropdown only
    public bool IsRequired { get; private set; }
    public int DisplayOrder { get; private set; }

    private static readonly string[] ValidFieldTypes =
        ["Text", "Number", "Dropdown", "Url", "Date"];

    private CustomFieldDefinition() { }

    private CustomFieldDefinition(
        Guid id,
        Guid projectId,
        string? taskType,
        string fieldName,
        string fieldType,
        string? options,
        bool isRequired,
        int displayOrder)
        : base(id)
    {
        ProjectId = projectId;
        TaskType = taskType;
        FieldName = fieldName;
        FieldType = fieldType;
        Options = options;
        IsRequired = isRequired;
        DisplayOrder = displayOrder;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<CustomFieldDefinition> Create(
        Guid projectId,
        string fieldName,
        string fieldType,
        int displayOrder,
        string? taskType = null,
        List<string>? options = null,
        bool isRequired = false)
    {
        if (!IsValidId(projectId))
            return CustomFieldErrors.ProjectIdRequired;

        if (string.IsNullOrWhiteSpace(fieldName))
            return CustomFieldErrors.FieldNameRequired;

        if (!IsValidFieldType(fieldType))
            return CustomFieldErrors.InvalidFieldType(fieldType);

        // Options only valid for Dropdown — required for Dropdown
        if (fieldType == "Dropdown" && (options is null || options.Count == 0))
            return CustomFieldErrors.OptionsRequiredForDropdown;

        if (fieldType != "Dropdown" && options is not null)
            return CustomFieldErrors.OptionsOnlyForDropdown;

        var serializedOptions = options is not null
            ? JsonSerializer.Serialize(options)
            : null;

        return new CustomFieldDefinition(
            id: Guid.NewGuid(),
            projectId: projectId,
            taskType: taskType,
            fieldName: fieldName.Trim(),
            fieldType: fieldType,
            options: serializedOptions,
            isRequired: isRequired,
            displayOrder: displayOrder
        );
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public List<string> GetOptions() =>
        Options is not null
            ? JsonSerializer.Deserialize<List<string>>(Options) ?? []
            : [];

    public bool AppliesToTaskType(string taskType) =>
        TaskType is null || TaskType.Equals(taskType, StringComparison.OrdinalIgnoreCase);

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private static bool IsValidFieldType(string ft) => ValidFieldTypes.Contains(ft);
}
