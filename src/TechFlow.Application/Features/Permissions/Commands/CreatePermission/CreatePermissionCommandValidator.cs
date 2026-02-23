using FluentValidation;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.Application.Features.Permissions.Commands.CreatePermission;

public sealed class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
{
    public CreatePermissionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Permission name is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxNameLength)
            .WithMessage($"Permission name cannot exceed {TechFlowConstants.Validation.MaxNameLength} characters.");

        RuleFor(x => x.Group)
            .NotEmpty().WithMessage("Permission group is required.")
            .Must(g => PermissionGroups.All.Contains(g, StringComparer.OrdinalIgnoreCase))
            .WithMessage($"Group must be one of: {string.Join(", ", PermissionGroups.All)}.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Permission description is required.")
            .MaximumLength(TechFlowConstants.Validation.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {TechFlowConstants.Validation.MaxDescriptionLength} characters.");
    }
}