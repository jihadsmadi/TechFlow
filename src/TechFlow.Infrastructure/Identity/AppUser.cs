using Microsoft.AspNetCore.Identity;

namespace TechFlow.Infrastructure.Identity;

public sealed class AppUser : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }

    public bool IsRefreshTokenValid(string token) =>
        RefreshToken == token &&
        RefreshTokenExpiresAt.HasValue &&
        RefreshTokenExpiresAt.Value > DateTimeOffset.UtcNow;
}