using FluentValidation;
using TechFlow.Domain.Common.Constants;

namespace TechFlow.Application.Features.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Company ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Company name is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Company name cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("Contact email is required.")
            .EmailAddress().WithMessage("Contact email is not valid.");

        RuleFor(x => x.Industry)
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Industry cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.")
            .When(x => x.Industry is not null);
    }
}