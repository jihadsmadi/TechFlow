using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Sprints.Commands.EndSprint;

public sealed record EndSprintCommand(
    Guid ProjectId,
    Guid SprintId,
    string IncompleteTasksAction) : IRequest<Result<Updated>>;
