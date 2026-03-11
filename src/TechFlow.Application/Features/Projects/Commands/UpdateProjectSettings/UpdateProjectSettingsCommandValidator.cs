using FluentValidation;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProjectSettings;

// ── Validator ─────────────────────────────────────────────────────────────────

public sealed class UpdateProjectSettingsCommandValidator
    : AbstractValidator<UpdateProjectSettingsCommand>
{
    private static readonly string[] ValidTaskTypes = ["Bug", "Feature", "Research", "Chore"];
    private static readonly string[] ValidPriorities = ["Low", "Medium", "High", "Critical"];

    public UpdateProjectSettingsCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();

        RuleFor(x => x.DefaultListNames)
            .NotEmpty()
            .Must(names => names.Count is >= 1 and <= 10)
            .WithMessage("Must provide between 1 and 10 list names.")
            .Must(names => names.All(n => !string.IsNullOrWhiteSpace(n)))
            .WithMessage("List names cannot be empty.");

        RuleFor(x => x.DefaultTaskType)
            .Must(t => ValidTaskTypes.Contains(t))
            .WithMessage("Invalid task type.");

        RuleFor(x => x.DefaultPriority)
            .Must(p => ValidPriorities.Contains(p))
            .WithMessage("Invalid priority.");
    }
}
