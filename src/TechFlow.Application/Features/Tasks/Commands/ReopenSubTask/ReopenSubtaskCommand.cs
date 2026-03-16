using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.ReopenSubTask;

public sealed record ReopenSubtaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid SubtaskId) : IRequest<Result<Updated>>;
