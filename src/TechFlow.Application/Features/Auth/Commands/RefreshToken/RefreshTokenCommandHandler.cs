using MediatR;
using Microsoft.IdentityModel.JsonWebTokens;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Application.Features.Auth.Commands.RefreshToken;

public sealed class RefreshTokenCommandHandler(
    IUnitOfWork   unitOfWork,
    IAuthService  authService,
    ITokenService tokenService)
    : IRequestHandler<RefreshTokenCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RefreshTokenCommand command,
        CancellationToken   ct)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(command.AccessToken);
        if (principal is null)
            return ApplicationErrors.InvalidAccessToken;

        var subClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(subClaim, out var domainUserId))
            return ApplicationErrors.InvalidAccessToken;

        var user = await unitOfWork.Users.GetByIdAsync(domainUserId, ct);
        if (user is null || !user.IsActive)
            return ApplicationErrors.InvalidAccessToken;

        var isValid = await authService.ValidateRefreshTokenAsync(
            user.IdentityUserId, command.RefreshToken, ct);

        if (!isValid)
            return ApplicationErrors.InvalidRefreshToken;

        var companyRoles = await unitOfWork.Users.GetCompanyRolesAsync(user.Id, ct);

        var (newRefreshToken,refreshExpiresAt) = await authService.GenerateAndStoreRefreshTokenAsync(
            user.IdentityUserId, ct);

        var (accessToken,expiresAt) = tokenService.GenerateAccessToken(
            user, companyRoles.RoleNames, companyRoles.Permissions);


        return new AuthResponse(
            AccessToken:  accessToken,
            RefreshToken: newRefreshToken,
            ExpiresAt:    expiresAt,
            RefreshTokenExpiresAt: refreshExpiresAt,
            User: new UserAuthDto(
                Id:        user.Id,
                Email:     user.Email,
                FirstName: user.FirstName,
                LastName:  user.LastName,
                FullName:  user.FullName,
                CompanyId: user.CompanyId,
                Role:      companyRoles.RoleNames.FirstOrDefault() ?? string.Empty));
    }
}
