using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Commands.MoveTask;

/// <summary>
/// Moves a task to a new list and/or position using fractional indexing.
/// 
/// Fractional indexing explained:
/// - Tasks have double DisplayOrder (e.g. 1.0, 2.0, 3.0)
/// - To insert between two tasks: newOrder = (prevOrder + nextOrder) / 2
/// - e.g. between 1.0 and 2.0 → 1.5
/// - e.g. between 1.0 and 1.5 → 1.25
/// - Only ONE task is updated — no shifting needed
/// 
/// The frontend sends PrevDisplayOrder and NextDisplayOrder of the
/// surrounding tasks. Backend calculates the new position.
/// If PrevDisplayOrder is null → task goes to top (nextOrder / 2)
/// If NextDisplayOrder is null → task goes to bottom (prevOrder + 1)
/// </summary>
public sealed record MoveTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid NewListId,
    double? PrevDisplayOrder,
    double? NextDisplayOrder) : IRequest<Result<Updated>>;
