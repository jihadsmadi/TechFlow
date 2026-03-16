using FluentValidation;


namespace TechFlow.Application.Features.Tasks.Commands.RenameSubTask;

public sealed class RenameSubtaskCommandValidator : AbstractValidator<RenameSubtaskCommand>
{
    public RenameSubtaskCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Subtask title is required.")
            .MaximumLength(100).WithMessage("Subtask title must not exceed 100 characters.");
    }
}
