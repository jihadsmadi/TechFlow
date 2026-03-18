using MediatR;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Queries.GetBacklog;

public sealed record GetBacklogQuery(
    Guid ProjectId) : IRequest<Result<IReadOnlyList<TaskSummaryDto>>>;
