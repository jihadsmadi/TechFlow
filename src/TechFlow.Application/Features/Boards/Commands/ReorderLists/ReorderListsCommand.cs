using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Boards.Commands.ReorderLists;

public sealed record ReorderListsCommand(
    Guid ProjectId,
    List<Guid> OrderedListIds) : IRequest<Result<Updated>>;
