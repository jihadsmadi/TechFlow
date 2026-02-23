using FluentValidation;
using TechFlow.Domain.Companies;

namespace TechFlow.Application.Features.Companies.Commands.SetFeatureFlag;

public sealed class SetFeatureFlagCommandValidator : AbstractValidator<SetFeatureFlagCommand>
{
    public SetFeatureFlagCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty().WithMessage("Company ID is required.");

        RuleFor(x => x.ToggledByUserId)
            .NotEmpty().WithMessage("Toggled by user ID is required.");

        RuleFor(x => x.FeatureKey)
            .NotEmpty().WithMessage("Feature key is required.")
            .Must(key => FeatureKeys.All.Contains(key))
            .WithMessage($"Feature key must be one of: {string.Join(", ", FeatureKeys.All)}.");
    }
}