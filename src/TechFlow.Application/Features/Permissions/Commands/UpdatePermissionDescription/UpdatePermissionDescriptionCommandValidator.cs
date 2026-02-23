using FluentValidation;
using TechFlow.Domain.Common.Constants;

namespace TechFlow.Application.Features.Permissions.Commands.UpdatePermissionDescription;

public sealed class UpdatePermissionDescriptionCommandValidator
    : AbstractValidator<UpdatePermissionDescriptionCommand>
{
    public UpdatePermissionDescriptionCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Permission ID is required.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {TechFlowConstants.Validation.MaxDescriptionLength} characters.");
    }
}