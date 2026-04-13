using MediatR;
using TechFlow.Application.Features.Invitations.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Invitaions.Commands.InviteUser;


public sealed record InviteUserCommand(
    Guid RoleId,
    string Email,
    Guid? ProjectId = null) : IRequest<Result<InvitationDto>>;
