using MediatR;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Queries.GetActiveSprints;
public sealed record GetActiveSprintQuery(
    Guid ProjectId) : IRequest<Result<SprintDto?>>;
