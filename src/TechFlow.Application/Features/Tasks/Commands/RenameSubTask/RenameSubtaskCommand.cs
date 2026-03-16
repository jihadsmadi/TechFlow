using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Commands.RenameSubTask;

public sealed record RenameSubtaskCommand(
    Guid ProjectId,
    Guid TaskId,
    Guid SubtaskId,
    string Title) : IRequest<Result<Updated>>;
