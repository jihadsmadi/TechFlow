using MediatR;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Invitations.Commands.AcceptInvitation;

public sealed record AcceptInvitationCommand(
    string Token,       
    string FirstName,
    string LastName,
    string Password,
    string ConfirmPassword) : IRequest<Result<AuthResponse>>;
