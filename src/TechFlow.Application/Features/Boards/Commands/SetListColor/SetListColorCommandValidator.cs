using FluentValidation;



namespace TechFlow.Application.Features.Boards.Commands.SetListColor;

public sealed class SetListColorCommandValidator : AbstractValidator<SetListColorCommand>
{
    public SetListColorCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.ListId).NotEmpty();
        RuleFor(x => x.Color)
            .Matches("^#[0-9a-fA-F]{6}$").WithMessage("Color must be a valid hex value.")
            .When(x => x.Color is not null);
    }
}
