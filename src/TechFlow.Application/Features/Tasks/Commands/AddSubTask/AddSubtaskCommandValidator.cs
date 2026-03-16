using FluentValidation;


namespace TechFlow.Application.Features.Tasks.Commands.AddSubtask;

public sealed class AddSubtaskCommandValidator : AbstractValidator<AddSubtaskCommand>
{
    public AddSubtaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Subtask title is required.")
            .MaximumLength(100).WithMessage("Subtask title must not exceed 100 characters.");
    }
}
