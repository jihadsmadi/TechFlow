using MediatR;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<Result<AuthResponse>>;