using FluentValidation;

namespace TechFlow.Application.Features.Sprints.Commands.UpdateSprint;

public sealed class UpdateSprintCommandValidator : AbstractValidator<UpdateSprintCommand>
{
    public UpdateSprintCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SprintId).NotEmpty();

        RuleFor(x => x.Name)
            .MaximumLength(100)
            .When(x => x.Name is not null);

        RuleFor(x => x.Goal)
            .MaximumLength(500)
            .When(x => x.Goal is not null);
    }
}
