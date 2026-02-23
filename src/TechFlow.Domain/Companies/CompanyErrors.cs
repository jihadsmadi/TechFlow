using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Companies;

public static class CompanyErrors
{
    // Name
    public static readonly Error NameRequired =
        Error.Validation("Company.NameRequired", "Company name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Company.NameTooLong", "Company name cannot exceed 100 characters.");

    // Contact
    public static readonly Error ContactEmailRequired =
        Error.Validation("Company.ContactEmailRequired", "Contact email is required.");

    // State
    public static readonly Error NotFound =
        Error.NotFound("Company.NotFound", "Company was not found.");

    public static readonly Error AlreadyInactive =
        Error.Conflict("Company.AlreadyInactive", "Company is already inactive.");

    public static readonly Error AlreadyActive =
        Error.Conflict("Company.AlreadyActive", "Company is already active.");

    // Settings
    public static readonly Error InvalidPrimaryColor =
        Error.Validation("Company.InvalidPrimaryColor", "Primary color must be a valid hex color (e.g. #3b82f6).");

    public static readonly Error SlugAlreadyExists =
        Error.Conflict("Company.SlugAlradyExists", "A Company with this slug already exists.");
    public static Error InvalidDateFormat(string format) =>
        Error.Validation("Company.InvalidDateFormat", $"'{format}' is not a valid date format. Valid options: DD/MM/YYYY, MM/DD/YYYY, YYYY-MM-DD.");

    public static Error InvalidLanguage(string language) =>
        Error.Validation("Company.InvalidLanguage", $"'{language}' is not a supported language. Supported: en, ar, fr.");

    // Feature flags
    public static Error InvalidFeatureKey(string key) =>
        Error.Validation("Company.InvalidFeatureKey", $"'{key}' is not a valid feature key.");
}