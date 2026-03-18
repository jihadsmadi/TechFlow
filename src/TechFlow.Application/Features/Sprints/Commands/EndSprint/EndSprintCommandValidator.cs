using FluentValidation;
using TechFlow.Domain.Projects.ProjectSettings;

namespace TechFlow.Application.Features.Sprints.Commands.EndSprint;

public sealed class EndSprintCommandValidator : AbstractValidator<EndSprintCommand>
{
    public EndSprintCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.SprintId).NotEmpty();

        RuleFor(x => x.IncompleteTasksAction)
            .NotEmpty()
            .Must(IncompleteTasksActionType.IsValid)
            .WithMessage($"Invalid action. Valid values: {string.Join(", ", IncompleteTasksActionType.All)}");
    }
}
