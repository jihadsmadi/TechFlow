using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.RemoveTaskFromSprint;

public sealed record RemoveTaskFromSprintCommand(
    Guid ProjectId,
    Guid SprintId,
    Guid TaskId) : IRequest<Result<Updated>>;
