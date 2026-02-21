using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Companies;

/// <summary>
/// Represents a single feature toggle for a company.
/// Collection owned by Company aggregate.
/// </summary>
public sealed class FeatureFlag
{
    public string FeatureKey { get; private set; } = string.Empty;
    public bool IsEnabled { get; private set; }
    public DateTime? EnabledAt { get; private set; }
    public Guid? EnabledByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private FeatureFlag() { }

    internal static FeatureFlag Create(string featureKey, bool isEnabled, Guid enabledByUserId)
    {
        return new FeatureFlag
        {
            FeatureKey = featureKey,
            IsEnabled = isEnabled,
            EnabledAt = isEnabled ? DateTime.UtcNow : null,
            EnabledByUserId = enabledByUserId,
            UpdatedAt = DateTime.UtcNow
        };
    }

    internal void SetEnabled(bool isEnabled, Guid toggledByUserId)
    {
        IsEnabled = isEnabled;
        EnabledAt = isEnabled ? DateTime.UtcNow : null;
        EnabledByUserId = toggledByUserId;
        UpdatedAt = DateTime.UtcNow;
    }
}

public static class FeatureKeys
{
    public const string CustomFields = "custom_fields";
    public const string Attachments = "attachments";
    public const string Subtasks = "subtasks";
    public const string ActivityLog = "activity_log";
    public const string GuestAccess = "guest_access";

    public static readonly IReadOnlyList<string> All =
    [
        CustomFields,
        Attachments,
        Subtasks,
        ActivityLog,
        GuestAccess
    ];
}