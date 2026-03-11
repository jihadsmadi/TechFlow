using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.UpdateProject;


public sealed record UpdateProjectCommand(
    Guid Id,
    string Name,
    string? Description,
    string? Color,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate) : IRequest<Result<Updated>>;
