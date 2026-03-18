using MediatR;
using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.CreateSprint;

public sealed record CreateSprintCommand(
    Guid ProjectId,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    string? Name = null,
    string? Goal = null) : IRequest<Result<SprintSummaryDto>>;
