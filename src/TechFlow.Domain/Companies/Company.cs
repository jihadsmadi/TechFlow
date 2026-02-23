using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies.Events;
using TechFlow.Domain.Companies.Modules;
using TechFlow.Domain.Companies.ValueObjects;

namespace TechFlow.Domain.Companies;

public sealed class Company : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public Slug Slug { get; private set; } = null!;
    public string ContactEmail { get; private set; } = string.Empty;
    public string? Industry { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Owned entities
    public CompanySettings Settings { get; private set; } = null!;

    private readonly List<FeatureFlag> _featureFlags = [];
    public IReadOnlyList<FeatureFlag> FeatureFlags => _featureFlags.AsReadOnly();

    private Company() { }

    private Company(Guid id, string name, Slug slug, string contactEmail, string? industry)
        : base(id)
    {
        Name = name;
        Slug = slug;
        ContactEmail = contactEmail;
        Industry = industry;
        Settings = CompanySettings.CreateDefault();
    }


    public static Result<Company> Create(string name, string slug, string contactEmail, string? industry = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CompanyErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return CompanyErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace(contactEmail))
            return CompanyErrors.ContactEmailRequired;

        var slugResult = Slug.Create(slug);
        if (slugResult.IsFailure)
            return slugResult.Errors;

        var company = new Company(
            id: Guid.NewGuid(),
            name: name.Trim(),
            slug: slugResult.Value,
            contactEmail: contactEmail.Trim().ToLower(),
            industry: industry?.Trim()
        );

        company.AddDomainEvent(new CompanyCreatedEvent(company.Id, company.Name, company.Slug.Value));

        return company;
    }


    public Result<Updated> Update(string name, string contactEmail, string? industry = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            return CompanyErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return CompanyErrors.NameTooLong;

        if (string.IsNullOrWhiteSpace(contactEmail))
            return CompanyErrors.ContactEmailRequired;

        Name = name.Trim();
        ContactEmail = contactEmail.Trim().ToLower();
        Industry = industry?.Trim();

        return Result.Updated;
    }

    public Result<Updated> UpdateSettings(
        string primaryColor, string? logoUrl, string? companyWebsite,
        string defaultTimezone, string dateFormat, string language,
        bool allowGuestAccess, bool requireTaskDueDate, bool allowMembersInvite)
    {
        var result = Settings.Update(
            primaryColor, logoUrl, companyWebsite,
            defaultTimezone, dateFormat, language,
            allowGuestAccess, requireTaskDueDate, allowMembersInvite);

        if (result.IsFailure)
            return result.Errors;

        return Result.Updated;
    }

    public Result<Updated> SetFeatureFlag(string featureKey, bool isEnabled, Guid toggledByUserId)
    {
        if (!FeatureKeys.All.Contains(featureKey))
            return CompanyErrors.InvalidFeatureKey(featureKey);

        var existing = _featureFlags.FirstOrDefault(f => f.FeatureKey == featureKey);

        if (existing is null)
            _featureFlags.Add(FeatureFlag.Create(featureKey, isEnabled, toggledByUserId));
        else
            existing.SetEnabled(isEnabled, toggledByUserId);

        AddDomainEvent(new FeatureFlagChangedEvent(Id, featureKey, isEnabled));
        return Result.Updated;
    }

    public bool IsFeatureEnabled(string featureKey) =>
        _featureFlags.FirstOrDefault(f => f.FeatureKey == featureKey)?.IsEnabled ?? false;

    public Result<Updated> Deactivate()
    {
        if (!IsActive)
            return CompanyErrors.AlreadyInactive;

        IsActive = false;
        AddDomainEvent(new CompanyDeactivatedEvent(Id));
        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive)
            return CompanyErrors.AlreadyActive;

        IsActive = true;
        return Result.Updated;
    }
}