using MediatR;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<Result<ProjectDto>>;
