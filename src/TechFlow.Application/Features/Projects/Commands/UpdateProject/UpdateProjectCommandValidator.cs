using FluentValidation;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProject;


public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(100).WithMessage("Project name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.Color)
            .Matches("^#[0-9a-fA-F]{6}$").WithMessage("Color must be a valid hex value.")
            .When(x => x.Color is not null);

        RuleFor(x => x)
            .Must(x => x.StartDate is null || x.EndDate is null || x.StartDate <= x.EndDate)
            .WithMessage("Start date must be before end date.");
    }
}
