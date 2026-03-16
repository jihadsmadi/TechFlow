using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.CompleteSubTask;

public sealed record CompleteSubtaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid SubtaskId) : IRequest<Result<Updated>>;
