using MediatR;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Queries.GetAllSprints;

public sealed record GetAllSprintsQuery(
    Guid ProjectId) : IRequest<Result<IReadOnlyList<SprintSummaryDto>>>;
