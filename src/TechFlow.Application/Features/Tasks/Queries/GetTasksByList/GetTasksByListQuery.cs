using MediatR;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Tasks.Queries.GetTasksByList;

public sealed record GetTasksByListQuery(
    Guid ProjectId,
    Guid ListId) : IRequest<Result<IReadOnlyList<TaskSummaryDto>>>;
