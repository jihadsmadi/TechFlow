using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Commands.DeleteTask;

public sealed record DeleteTaskCommand(
    Guid ProjectId,
    Guid TaskId) : IRequest<Result<Deleted>>;
