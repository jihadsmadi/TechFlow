using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Companies.Modules;

/// <summary>
/// Owned entity — lives inside the Company aggregate.
/// Stored in COMPANY_SETTINGS table via EF Core owned entity configuration.
/// </summary>
public sealed class CompanySettings
{
    public string PrimaryColor { get; private set; } = "#3b82f6";
    public string? LogoUrl { get; private set; }
    public string? CompanyWebsite { get; private set; }
    public string DefaultTimezone { get; private set; } = "UTC";
    public string DateFormat { get; private set; } = "DD/MM/YYYY";
    public string Language { get; private set; } = "en";
    public bool AllowGuestAccess { get; private set; } = false;
    public bool RequireTaskDueDate { get; private set; } = false;
    public bool AllowMembersInvite { get; private set; } = false;

    private static readonly string[] ValidDateFormats = ["DD/MM/YYYY", "MM/DD/YYYY", "YYYY-MM-DD"];
    private static readonly string[] ValidLanguages = ["en", "ar", "fr"];

    // EF Core
    private CompanySettings() { }

    internal static CompanySettings CreateDefault() => new();

    public Result<Updated> Update(
        string primaryColor,
        string? logoUrl,
        string? companyWebsite,
        string defaultTimezone,
        string dateFormat,
        string language,
        bool allowGuestAccess,
        bool requireTaskDueDate,
        bool allowMembersInvite)
    {
        if (string.IsNullOrWhiteSpace(primaryColor) || !primaryColor.StartsWith('#') || primaryColor.Length != 7)
            return CompanyErrors.InvalidPrimaryColor;

        if (!ValidDateFormats.Contains(dateFormat))
            return CompanyErrors.InvalidDateFormat(dateFormat);

        if (!ValidLanguages.Contains(language))
            return CompanyErrors.InvalidLanguage(language);

        PrimaryColor = primaryColor;
        LogoUrl = logoUrl?.Trim();
        CompanyWebsite = companyWebsite?.Trim();
        DefaultTimezone = defaultTimezone.Trim();
        DateFormat = dateFormat;
        Language = language;
        AllowGuestAccess = allowGuestAccess;
        RequireTaskDueDate = requireTaskDueDate;
        AllowMembersInvite = allowMembersInvite;

        return Result.Updated;
    }
}