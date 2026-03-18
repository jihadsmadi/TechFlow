using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.UpdateSprint;

public sealed record UpdateSprintCommand(
    Guid ProjectId,
    Guid SprintId,
    string? Name = null,
    string? Goal = null,
    DateTimeOffset? StartDate = null,
    DateTimeOffset? EndDate = null) : IRequest<Result<Updated>>;
