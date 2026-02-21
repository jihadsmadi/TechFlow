using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Companies.ValueObjects;

public static class SlugErrors
{
    public static readonly Error Required =
        Error.Validation("Slug.Required", "Company slug is required.");

    public static readonly Error TooShort =
        Error.Validation("Slug.TooShort", "Slug must be at least 3 characters.");

    public static readonly Error TooLong =
        Error.Validation("Slug.TooLong", "Slug cannot exceed 60 characters.");

    public static readonly Error InvalidFormat =
        Error.Validation("Slug.InvalidFormat", "Slug must contain only lowercase letters, numbers, and hyphens (e.g. techflow-solutions).");

    public static readonly Error AlreadyTaken =
        Error.Conflict("Slug.AlreadyTaken", "This slug is already taken. Please choose a different one.");
}