using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Common.Interfaces.Services;

public interface IAuthService
{
    Task<Result<Guid>> CreateIdentityUserAsync(
        string email, string password, CancellationToken ct = default);

    Task<Result<Guid>> ValidateCredentialsAsync(
        string email, string password, CancellationToken ct = default);
    Task<(string,DateTimeOffset)> GenerateAndStoreRefreshTokenAsync(
        Guid identityUserId, CancellationToken ct = default);

    Task<bool> ValidateRefreshTokenAsync(
        Guid identityUserId, string refreshToken, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
}