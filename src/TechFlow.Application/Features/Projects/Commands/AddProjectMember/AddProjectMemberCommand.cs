using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.AddProjectMember;

public sealed record AddProjectMemberCommand(
    Guid ProjectId,
    Guid UserId) : IRequest<Result<Created>>;


