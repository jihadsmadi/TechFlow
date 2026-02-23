namespace TechFlow.Application.Features.Companies.Dtos
{
    public sealed class FeatureFlagDto
    {
            public string FeatureKey { get; init; } = string.Empty;
            public bool IsEnabled { get; init; }
            public DateTimeOffset? EnabledAt { get; init; }  
            public Guid? EnabledByUserId { get; init; }  
    }
}