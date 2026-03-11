using MediatR;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Users;

namespace TechFlow.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IUnitOfWork   unitOfWork,
    IAuthService  authService,
    ITokenService tokenService)
    : IRequestHandler<LoginCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        LoginCommand      command,
        CancellationToken ct)
    {
        var credentialsResult = await authService.ValidateCredentialsAsync(
            command.Email, command.Password, ct);

        if (credentialsResult.IsFailure)
            return credentialsResult.Errors;

        var identityUserId = credentialsResult.Value;

        var user = await unitOfWork.Users.GetByIdentityIdAsync(identityUserId, ct);
        if (user is null)
            return UserErrors.NotFound;

        if (!user.IsActive)
            return Error.Forbidden(
                "Auth.AccountDeactivated",
                "Your account has been deactivated.");

        var companyRoles = await unitOfWork.Users.GetCompanyRolesAsync(user.Id, ct);

        var (refreshToken,refreshExpiresAt) = await authService.GenerateAndStoreRefreshTokenAsync(
            identityUserId, ct);

        var (accessToken,expiresAt) = tokenService.GenerateAccessToken(
            user, companyRoles.RoleNames, companyRoles.Permissions);


        return new AuthResponse(
            AccessToken:  accessToken,
            RefreshToken: refreshToken,
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
