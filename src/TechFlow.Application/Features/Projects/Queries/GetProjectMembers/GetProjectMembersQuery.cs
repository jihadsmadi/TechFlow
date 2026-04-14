using MediatR;
using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Queries.GetProjectMembers;

public sealed record GetProjectMembersQuery(Guid ProjectId)
    : IRequest<Result<List<ProjectMemberWithUserDto>>>;
