using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Projects.Commands.RemoveProjectMember;

public sealed record RemoveProjectMemberCommand(
    Guid ProjectId,
    Guid UserId) : IRequest<Result<Deleted>>;
