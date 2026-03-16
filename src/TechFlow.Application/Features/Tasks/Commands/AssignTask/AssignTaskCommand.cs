using MediatR;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Tasks.Commands.AssignTask;

public sealed record AssignTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid UserId) : IRequest<Result<Updated>>;
