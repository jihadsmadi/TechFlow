using FluentValidation;


namespace TechFlow.Application.Features.Boards.Commands.AddList;

public sealed class AddListCommandValidator : AbstractValidator<AddListCommand>
{
    public AddListCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("List name is required.")
            .MaximumLength(100).WithMessage("List name must not exceed 100 characters.");
        RuleFor(x => x.Color)
            .Matches("^#[0-9a-fA-F]{6}$").WithMessage("Color must be a valid hex value.")
            .When(x => x.Color is not null);
    }
}
