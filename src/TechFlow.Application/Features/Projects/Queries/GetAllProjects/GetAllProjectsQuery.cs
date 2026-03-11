using MediatR;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Queries.GetAllProjects;

public sealed record GetAllProjectsQuery(
    bool IncludeArchived = false) : IRequest<Result<List<ProjectSummaryDto>>>;
