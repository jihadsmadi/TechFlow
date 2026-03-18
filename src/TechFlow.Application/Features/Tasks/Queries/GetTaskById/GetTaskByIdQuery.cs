using MediatR;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Queries.GetTaskById;

public sealed record GetTaskByIdQuery(
    Guid ProjectId,
    Guid TaskId) : IRequest<Result<TaskDto>>;
