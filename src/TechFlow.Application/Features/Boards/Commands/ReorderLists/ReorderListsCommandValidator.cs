using FluentValidation;


namespace TechFlow.Application.Features.Boards.Commands.ReorderLists;

public sealed class ReorderListsCommandValidator : AbstractValidator<ReorderListsCommand>
{
    public ReorderListsCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.OrderedListIds)
            .NotEmpty().WithMessage("List order cannot be empty.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Duplicate list IDs are not allowed.");
    }
}
