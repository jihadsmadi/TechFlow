using System.Text.RegularExpressions;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Companies.ValueObjects;

/// <summary>
/// URL-friendly unique identifier for a company.
/// Rules: lowercase, letters/digits/hyphens only, 3–60 chars.
/// Example: "techflow-solutions"
/// </summary>
public sealed class Slug
{
    public string Value { get; }

    private static readonly Regex ValidPattern = new(@"^[a-z0-9]+(?:-[a-z0-9]+)*$", RegexOptions.Compiled);

    private Slug(string value) => Value = value;

    public static Result<Slug> Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return SlugErrors.Required;

        var normalized = raw.Trim().ToLower()
            .Replace(" ", "-")
            .Replace("_", "-");

        if (normalized.Length < 3)
            return SlugErrors.TooShort;

        if (normalized.Length > 60)
            return SlugErrors.TooLong;

        if (!ValidPattern.IsMatch(normalized))
            return SlugErrors.InvalidFormat;

        return new Slug(normalized);
    }

    public override string ToString() => Value;
    public override bool Equals(object? obj) => obj is Slug other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
