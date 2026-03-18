using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.StartSprint;

public sealed record StartSprintCommand(
    Guid ProjectId,
    Guid SprintId) : IRequest<Result<Updated>>;
