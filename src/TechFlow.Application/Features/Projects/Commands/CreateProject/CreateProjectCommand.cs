using MediatR;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.CreateProject;


public sealed record CreateProjectCommand(
    string Name,
    string? Description,
    string? Color,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate) : IRequest<Result<ProjectSummaryDto>>;
