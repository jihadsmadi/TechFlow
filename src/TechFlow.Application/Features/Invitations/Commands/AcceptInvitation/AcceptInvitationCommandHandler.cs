using MediatR;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Application.Features.Auth.Dtos;
using TechFlow.Application.Features.Invitations.Commands.AcceptInvitation;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Invitations;
using TechFlow.Domain.Users;
using TechFlow.Domain.Users.UserCompanyRoles;

public sealed class AcceptInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    IAuthService authService,
    ITokenService tokenService)
    : IRequestHandler<AcceptInvitationCommand, Result<AuthResponse>>
{
    public async Task<Result<AuthResponse>> Handle(
        AcceptInvitationCommand command,
        CancellationToken ct)
    {
        var tokenHash = Invitation.HashToken(command.Token);
        var invitation = await unitOfWork.Invitations.GetByTokenHashAsync(tokenHash, ct);

        if (invitation is null || !invitation.IsValid())
            return InvitationErrors.NotFound;

        var existingUser = await unitOfWork.Users.GetByEmailAsync(invitation.Email, ct);

        User domainUser;

        if (existingUser is not null)
        {
            if (existingUser.CompanyId == invitation.CompanyId)
                return InvitationErrors.UserAlreadyMember;

            domainUser = existingUser;
        }
        else
        {
            var identityResult = await authService.CreateIdentityUserAsync(
                invitation.Email,
                command.Password,
                ct);

            if (identityResult.IsFailure)
                return identityResult.TopError;

            var userResult = User.Create(
                identityUserId: identityResult.Value,
                companyId: invitation.CompanyId,
                firstName: command.FirstName,
                lastName: command.LastName,
                email: invitation.Email);

            if (userResult.IsFailure)
                return userResult.TopError;

            domainUser = userResult.Value;
            unitOfWork.Users.Add(domainUser);
        }

        var companyRole = UserCompanyRole.Create(
            domainUser.Id,
            invitation.CompanyId,
            invitation.RoleId);

        if (companyRole.IsFailure)
            return companyRole.TopError;

        var assignResult = domainUser.AssignCompanyRole(companyRole.Value);
        if (assignResult.IsFailure)
            return assignResult.TopError;

        if (invitation.ProjectId.HasValue)
        {
            var project = await unitOfWork.Projects
                .GetByIdWithMembersAsync(invitation.ProjectId.Value, ct);

            if (project is null)
                return InvitationErrors.NotFound;

            var addResult = project.AddMember(domainUser.Id, domainUser.Id);
            if (addResult.IsFailure)
                return addResult.TopError;
        }

        var useResult = invitation.MarkAsUsed(domainUser.Id);
        if (useResult.IsFailure)
            return useResult.TopError;

        await unitOfWork.SaveChangesAsync(ct);

        var rolesData = await unitOfWork.Users
            .GetCompanyRolesAsync(domainUser.Id, ct);

        var (accessToken, accessExpires) = tokenService.GenerateAccessToken(
            domainUser,
            rolesData.RoleNames,
            rolesData.Permissions);

        var (refreshToken, refreshExpires) =
            await authService.GenerateAndStoreRefreshTokenAsync(
                domainUser.IdentityUserId,
                ct);

        return new AuthResponse(
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            ExpiresAt: accessExpires,
            RefreshTokenExpiresAt: refreshExpires,
            User: new UserAuthDto(
                domainUser.Id,
                domainUser.Email,
                domainUser.FirstName,
                domainUser.LastName,
                domainUser.FullName,
                domainUser.CompanyId,
                rolesData.RoleNames.FirstOrDefault() ?? string.Empty)
        );
    }
}