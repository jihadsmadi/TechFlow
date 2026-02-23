using FluentValidation;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Companies.Modules;

namespace TechFlow.Application.Features.Companies.Commands.UpdateCompanySettings;

public sealed class UpdateCompanySettingsCommandValidator
    : AbstractValidator<UpdateCompanySettingsCommand>
{
    public UpdateCompanySettingsCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required.");

        // Domain validates hex format — FluentValidation catches empty
        RuleFor(x => x.PrimaryColor)
            .NotEmpty().WithMessage("Primary color is required.");

        RuleFor(x => x.DefaultTimezone)
            .NotEmpty().WithMessage("Default timezone is required.");

        // Domain validates accepted values — FluentValidation catches empty
        RuleFor(x => x.DateFormat)
            .NotEmpty().WithMessage("Date format is required.")
            .Must(f => CompanySettings.ValidDateFormat.Contains(f))
            .WithMessage($"Date format must be one of: {string.Join(", ", CompanySettings.ValidDateFormat)}.");

        RuleFor(x => x.Language)
            .NotEmpty().WithMessage("Language is required.")
            .Must(l => CompanySettings.ValidLanguagesList.Contains(l))
            .WithMessage($"Language must be one of: {string.Join(", ", CompanySettings.ValidLanguagesList)}.");

        RuleFor(x => x.LogoUrl)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Logo URL must be a valid URL.")
            .When(x => x.LogoUrl is not null);

        RuleFor(x => x.CompanyWebsite)
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Company website must be a valid URL.")
            .When(x => x.CompanyWebsite is not null);
    }
}