using MediatR;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Commands.CreateTask;

public sealed record CreateTaskCommand(
    Guid ProjectId,
    Guid ListId,
    string Title,
    string? Description = null,
    string? Priority = null,
    string? Type = null,
    DateTimeOffset? DueDate = null,
    int? EstimatedMinutes = null) : IRequest<Result<TaskSummaryDto>>;
