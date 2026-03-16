using FluentValidation;

namespace TechFlow.Application.Features.Tasks.Commands.MoveTask;

public sealed class MoveTaskCommandValidator : AbstractValidator<MoveTaskCommand>
{
    public MoveTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.TaskId).NotEmpty();
        RuleFor(x => x.NewListId).NotEmpty();
    }
}
