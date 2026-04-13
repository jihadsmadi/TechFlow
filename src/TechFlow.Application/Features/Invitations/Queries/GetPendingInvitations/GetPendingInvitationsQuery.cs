using MediatR;
using TechFlow.Application.Features.Invitations.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Invitaions.Queries.GetPendingInvitations;

public sealed record GetPendingInvitationsQuery 
        : IRequest<Result<IReadOnlyList<InvitationDto>>>;
