using FluentValidation;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class UpdateProjectSettingsCommandValidator
    : AbstractValidator<UpdateProjectSettingsCommand>
{
    private static readonly string[] ValidTaskTypes = ["Bug", "Feature", "Research", "Chore"];
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High", "Critical"];
    private static readonly string[] ValidIncompleteActions = ["MoveToBacklog", "MoveToNextSprint", "LeaveInPlace"];

    public UpdateProjectSettingsCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();

        When(x => x.DefaultListNames is not null, () =>
        {
            RuleFor(x => x.DefaultListNames)
                .Must(names => names!.Count is >= 1 and <= 10)
                .WithMessage("Must provide between 1 and 10 list names.")
                .Must(names => names!.All(n => !string.IsNullOrWhiteSpace(n)))
                .WithMessage("List names cannot be empty.");
        });

        When(x => x.DefaultTaskType is not null, () =>
        {
            RuleFor(x => x.DefaultTaskType)
                .Must(t => ValidTaskTypes.Contains(t))
                .WithMessage($"Invalid task type. Valid values: {string.Join(", ", ValidTaskTypes)}");
        });

        When(x => x.DefaultPriority is not null, () =>
        {
            RuleFor(x => x.DefaultPriority)
                .Must(p => ValidPriorities.Contains(p))
                .WithMessage($"Invalid priority. Valid values: {string.Join(", ", ValidPriorities)}");
        });

        When(x => x.SprintDurationDays.HasValue, () =>
        {
            RuleFor(x => x.SprintDurationDays)
                .GreaterThanOrEqualTo(1)
                .LessThanOrEqualTo(30)
                .WithMessage("Sprint duration must be between 1 and 30 days.");
        });

        When(x => x.IncompleteTasksAction is not null, () =>
        {
            RuleFor(x => x.IncompleteTasksAction)
                .Must(a => ValidIncompleteActions.Contains(a))
                .WithMessage($"Invalid incomplete tasks action. Valid values: {string.Join(", ", ValidIncompleteActions)}");
        });
    }
}