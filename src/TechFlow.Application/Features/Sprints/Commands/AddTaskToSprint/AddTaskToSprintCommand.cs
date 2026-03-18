using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.AddTaskToSprint;

public sealed record AddTaskToSprintCommand(
    Guid ProjectId,
    Guid SprintId,
    Guid TaskId) : IRequest<Result<Updated>>;
