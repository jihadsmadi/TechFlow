namespace TechFlow.Infrastructure.Settings;

public sealed class ClientSettings
{
    public const string SectionName = "Client";

    public string WebBaseUrl { get; init; } = string.Empty;
}