using FluentValidation;

namespace TechFlow.Application.Features.Tasks.Commands.CreateTask;

public sealed class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
        RuleFor(x => x.EstimatedMinutes)
            .GreaterThan(0).WithMessage("Estimated minutes must be greater than 0.")
            .When(x => x.EstimatedMinutes.HasValue);
    }
}
