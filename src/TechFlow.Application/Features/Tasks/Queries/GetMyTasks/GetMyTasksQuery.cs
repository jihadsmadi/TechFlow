using MediatR;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Queries.GetMyTasks;

public sealed record GetMyTasksQuery(
    bool IncludeCompleted = false) : IRequest<Result<IReadOnlyList<MyTaskDto>>>;