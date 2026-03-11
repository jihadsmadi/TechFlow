namespace TechFlow.Application.Features.Auth.Dtos;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset ExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    UserAuthDto User);
