using FluentValidation;
using TechFlow.Domain.Common.Constants;

namespace TechFlow.Application.Features.Roles.Commands.UpdateRole;

public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Role ID is required.");

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