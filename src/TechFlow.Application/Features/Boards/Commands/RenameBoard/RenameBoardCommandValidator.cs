using FluentValidation;

namespace TechFlow.Application.Features.Boards.Commands.RenameBoard;

public sealed class RenameBoardCommandValidator : AbstractValidator<RenameBoardCommand>
{
    public RenameBoardCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Board name is required.")
            .MaximumLength(100).WithMessage("Board name must not exceed 100 characters.");
    }
}
