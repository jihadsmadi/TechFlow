using MediatR;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Queries.GetSprintById;

public sealed record GetSprintByIdQuery(
    Guid ProjectId,
    Guid SprintId) : IRequest<Result<SprintDto>>;
