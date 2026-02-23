using TechFlow.Application.Features.Companies.Dtos;
using TechFlow.Application.Features.Permissions.Mappers;
using TechFlow.Application.Features.Roles.DTOs;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Companies.Mappers;

public static class CompanyMapper
{
    public static CompanyDto ToDto(this Company entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Slug = entity.Slug.Value,
        ContactEmail = entity.ContactEmail,
        Industry = entity.Industry!,
        IsActive = entity.IsActive,
        Settings = new CompanySettingsDto
        {
            PrimaryColor = entity.Settings.PrimaryColor,
            LogoUrl = entity.Settings.LogoUrl,
            CompanyWebsite = entity.Settings.CompanyWebsite,
            DefaultTimezone = entity.Settings.DefaultTimezone,
            DateFormat = entity.Settings.DateFormat,
            Language = entity.Settings.Language,
            AllowGuestAccess = entity.Settings.AllowGuestAccess,
            RequireTaskDueDate = entity.Settings.RequireTaskDueDate,
            AllowMembersInvite = entity.Settings.AllowMembersInvite
        },
        FeatureFlags = entity.FeatureFlags.Select(ff => new FeatureFlagDto
        {
            FeatureKey = ff.FeatureKey,
            IsEnabled = ff.IsEnabled
        }).ToList()
    };

    public static CompanySummaryDto ToSummaryDto(this Company entity) => new()
    {
           Id = entity.Id,
           Name = entity.Name,
           Slug = entity.Slug.Value,
           ContactEmail = entity.ContactEmail,
           Industry = entity.Industry!,
           IsActive = entity.IsActive,
           FeatureFlagsCount = entity.FeatureFlags.Count

    };
    public static List<CompanyDto> ToDtos(this IEnumerable<Company> entities) =>
        [.. entities.Select(e => e.ToDto())];

    public static List<CompanySummaryDto> ToSummaryDtos(this IEnumerable<Company> entities) =>
        [.. entities.Select(e => e.ToSummaryDto())];

}