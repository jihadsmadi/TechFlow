using MediatR;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Invitaions.Commands.RevokeInvitaion;


public sealed record RevokeInvitationCommand(
    Guid InvitationId) : IRequest<Result<Updated>>;
