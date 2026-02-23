using FluentValidation;
using TechFlow.Domain.Common.Constants;

namespace TechFlow.Application.Features.Roles.Commands.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Role name is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Role name cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Role description is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {TechFlowConstants.Validation.MaxDescriptionLength} characters.");
    }
}