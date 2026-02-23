using FluentValidation;
using TechFlow.Domain.Common.Constants;

namespace TechFlow.Application.Features.Companies.Commands.CreateCompany;

public sealed class CreateCompanyCommandValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Company name cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.");

        // Slug: basic shape only — domain validates full format
        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MinimumLength(3).WithMessage("Slug must be at least 3 characters.")
            .MaximumLength(60).WithMessage("Slug cannot exceed 60 characters.")
            .Matches("^[a-z0-9-]+$").WithMessage("Slug can only contain lowercase letters, numbers, and hyphens.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Contact email is not valid.");

        RuleFor(x => x.Industry)
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Industry cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.")
            .When(x => x.Industry is not null);
    }
}