using TechFlow.Domain.Common;

namespace TechFlow.Domain.Companies.Events;

public class FeatureFlagChangedEvent : DomainEvent
{
    public Guid CompanyId { get; }
    public string FeatureKey { get; } = string.Empty;
    public bool IsEnabled { get; }

    public FeatureFlagChangedEvent(Guid companyId, string featureKey, bool isEnabled)
    {
        CompanyId = companyId;
        FeatureKey = featureKey;
        IsEnabled = isEnabled;
    }
}