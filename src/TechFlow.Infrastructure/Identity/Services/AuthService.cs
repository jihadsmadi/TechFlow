using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Domain.Common.Results;
using TechFlow.Infrastructure.Settings;

namespace TechFlow.Infrastructure.Identity.Services;


public sealed class AuthService(
    UserManager<AppUser> userManager,
    IOptions<JwtSettings> jwtOptions)
    : IAuthService
{
    private readonly JwtSettings _jwt = jwtOptions.Value;

    public async Task<Result<Guid>> CreateIdentityUserAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var appUser = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = email,
            Email = email,
            EmailConfirmed = true   // skip email confirmation for now
        };

        var result = await userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            var err = result.Errors.First();
            return Error.Failure($"Identity.{err.Code}", err.Description);
        }

        return appUser.Id;
    }

    public async Task<Result<Guid>> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser is null)
            return ApplicationErrors.InvalidCredentials;

        if (!await userManager.CheckPasswordAsync(appUser, password))
            return ApplicationErrors.InvalidCredentials;

        return appUser.Id;
    }

    public async Task<(string,DateTimeOffset)> GenerateAndStoreRefreshTokenAsync(
        Guid identityUserId,
        CancellationToken ct = default)
    {
        var appUser = await userManager.FindByIdAsync(identityUserId.ToString())
            ?? throw new InvalidOperationException(
                $"AppUser {identityUserId} not found when generating refresh token.");

        var token = GenerateToken();
        var expires = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenExpirationDays);

        appUser.RefreshToken = token;
        appUser.RefreshTokenExpiresAt = expires;
        await userManager.UpdateAsync(appUser);

        return (token,expires);
    }

    public async Task<bool> ValidateRefreshTokenAsync(
        Guid identityUserId,
        string refreshToken,
        CancellationToken ct = default)
    {
        var appUser = await userManager.FindByIdAsync(identityUserId.ToString());
        return appUser is not null && appUser.IsRefreshTokenValid(refreshToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        var result = await userManager.FindByEmailAsync(email);
        if (result is not null)
            return true;

        return false;
    }

    private static string GenerateToken()
        => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}