using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    Guid ProjectId,
    Guid TaskId,
    string Title,
    string? Description = null,
    string? Priority = null,
    string? Type = null,
    DateTimeOffset? DueDate = null,
    int? EstimatedMinutes = null) : IRequest<Result<Updated>>;
