using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.CancelSprint;

public sealed record CancelSprintCommand(
    Guid ProjectId,
    Guid SprintId) : IRequest<Result<Updated>>;
