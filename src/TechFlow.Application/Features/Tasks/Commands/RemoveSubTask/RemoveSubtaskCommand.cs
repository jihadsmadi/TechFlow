using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.RemoveSubtask;

public sealed record RemoveSubtaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid SubtaskId) : IRequest<Result<Deleted>>;
