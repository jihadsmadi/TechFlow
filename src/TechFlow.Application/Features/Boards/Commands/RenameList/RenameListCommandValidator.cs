using FluentValidation;


namespace TechFlow.Application.Features.Boards.Commands.RenameList;

public sealed class RenameListCommandValidator : AbstractValidator<RenameListCommand>
{
    public RenameListCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("List name is required.")
            .MaximumLength(100).WithMessage("List name must not exceed 100 characters.");
    }
}
