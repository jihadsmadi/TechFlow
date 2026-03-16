using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.UnassignTask;

public sealed record UnassignTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid UserId) : IRequest<Result<Updated>>;
