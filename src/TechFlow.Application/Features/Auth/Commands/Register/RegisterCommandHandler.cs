using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Users;
using TechFlow.Domain.Users.UserCompanyRoles;

namespace TechFlow.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IUnitOfWork   unitOfWork,
    IAuthService  authService,
    ITokenService tokenService)
    : IRequestHandler<RegisterCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        RegisterCommand   command,
        CancellationToken ct)
    {
        if (await authService.EmailExistsAsync(command.Email, ct))
            return UserErrors.EmailAlreadyExists;

        if (await unitOfWork.Users.ExistsByEmailAsync(command.Email, ct))
            return UserErrors.EmailAlreadyExists;

        if (await unitOfWork.Companies.ExistsBySlugAsync(command.CompanySlug, ct))
            return CompanyErrors.SlugAlreadyExists;

        var companyResult = Company.Create(
            command.CompanyName,
            command.CompanySlug,
            command.CompanyEmail,
            command.Industry);

        if (companyResult.IsFailure) return companyResult.Errors;

        var company = companyResult.Value;
        unitOfWork.Companies.Add(company);

        var identityResult = await authService.CreateIdentityUserAsync(
            command.Email, command.Password, ct);

        if (identityResult.IsFailure) return identityResult.Errors;

        var identityUserId = identityResult.Value;

        var userResult = User.Create(
            identityUserId: identityUserId,
            companyId:      company.Id,
            firstName:      command.FirstName,
            lastName:       command.LastName,
            email:          command.Email);

        if (userResult.IsFailure) return userResult.Errors;

        var user = userResult.Value;
        unitOfWork.Users.Add(user);

        var adminRole = await unitOfWork.Roles.GetByNameAsync(SystemRoles.Admin, ct);
        if (adminRole is null)
            return Error.Failure("Role.AdminNotFound",
                "Admin role not found. Ensure seed data is applied.");

        var companyRoleResult = UserCompanyRole.Create(
            userId:           user.Id,
            roleId:           adminRole.Id,
            assignedByUserId: user.Id);   // self-assigned on registration

        if (companyRoleResult.IsFailure) return companyRoleResult.Errors;

        var assignResult = user.AssignCompanyRole(companyRoleResult.Value);
        if (assignResult.IsFailure) return assignResult.Errors;

        await unitOfWork.SaveChangesAsync(ct);

        var (refreshToken,refreshExpiresAt) = await authService.GenerateAndStoreRefreshTokenAsync(
            identityUserId, ct);

        var roles       = new[] { adminRole.Name };
        var permissions = adminRole.Permissions.Select(p => p.Name);
        var (accessToken,expiresAt) = tokenService.GenerateAccessToken(user, roles, permissions);
        

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
                Role:      adminRole.Name));
    }
}
