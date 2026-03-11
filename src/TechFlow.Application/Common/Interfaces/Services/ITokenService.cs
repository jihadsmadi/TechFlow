using System.Security.Claims;
using TechFlow.Domain.Users;

namespace TechFlow.Application.Common.Interfaces.Services;

public interface ITokenService
{
    (string,DateTimeOffset) GenerateAccessToken(
        User user, IEnumerable<string> roles, IEnumerable<string> permissions);

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string accessToken);

    int AccessTokenExpirationHours { get; }
    int RefreshTokenExpirationDays { get; }
}