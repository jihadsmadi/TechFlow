using MediatR;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Auth.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthResponse>>;