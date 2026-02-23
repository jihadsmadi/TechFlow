using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.CustomeFields;

/// <summary>
/// The actual value stored for a custom field on a specific task.
/// e.g. Task #42's "Severity" field = "Critical"
/// </summary>
public sealed class CustomFieldValue : Entity
{
    public Guid TaskId { get; private set; }
    public Guid CustomFieldDefinitionId { get; private set; }
    public string Value { get; private set; } = string.Empty;

    private CustomFieldValue() { }

    private CustomFieldValue(Guid id, Guid taskId, Guid customFieldDefinitionId, string value)
        : base(id)
    {
        TaskId = taskId;
        CustomFieldDefinitionId = customFieldDefinitionId;
        Value = value;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    internal static Result<CustomFieldValue> Create(
        Guid taskId,
        Guid customFieldDefinitionId,
        string value)
    {
        if (!IsValidId(taskId))
            return CustomFieldErrors.TaskIdRequired;

        if (!IsValidId(customFieldDefinitionId))
            return CustomFieldErrors.DefinitionIdRequired;

        if (string.IsNullOrWhiteSpace(value))
            return CustomFieldErrors.ValueRequired;

        return new CustomFieldValue(Guid.NewGuid(), taskId, customFieldDefinitionId, value.Trim());
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> UpdateValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return CustomFieldErrors.ValueRequired;

        Value = value.Trim();
        return Result.Updated;
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}