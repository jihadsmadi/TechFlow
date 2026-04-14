using MediatR;
using Microsoft.Extensions.Options;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Application.Features.Invitations.Dtos;
using TechFlow.Application.Features.Invitations.Mappers;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Invitations;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;

namespace TechFlow.Application.Features.Invitaions.Commands.InviteUser;


public sealed class InviteUserCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser,
    IEmailService emailService,
    IInvitationLinkBuilder appUrlService)
    : IRequestHandler<InviteUserCommand, Result<InvitationDto>>
{
    public async Task<Result<InvitationDto>> Handle(
        InviteUserCommand command,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var companyId = currentUser.CompanyId;

        // verify role exists
        var role = await unitOfWork.Roles.GetByIdAsync(command.RoleId, ct);
        if (role is null)
            return RoleErrors.NotFound;

        // if project-scoped invite — verify project exists and belongs to company
        if (command.ProjectId.HasValue)
        {
            var project = await unitOfWork.Projects.GetByIdAsync(command.ProjectId.Value, ct);
            if (project is null || project.CompanyId != companyId)
                return ProjectErrors.NotFound;
        }

        // if pending invite already exists — revoke it first
        var existing = await unitOfWork.Invitations.GetPendingByEmailAsync(
            companyId, command.Email, ct);

        if (existing is not null)
        {
            var revokeResult = existing.Revoke();
            if (revokeResult.IsFailure)
                return revokeResult.TopError;
        }

        // check if user is already a member of the company
        var existingUser = await unitOfWork.Users.GetByEmailAsync(command.Email, ct);
        if (existingUser is not null)
            return InvitationErrors.UserAlreadyMember;

       

        // create invitation
        var createResult = Invitation.Create(
            companyId: companyId,
            invitedByUserId: currentUser.Id.Value,
            roleId: command.RoleId,
            email: command.Email,
            projectId: command.ProjectId);

        if (createResult.IsFailure)
            return createResult.TopError;

        var (invitation, rawToken) = createResult.Value;

        unitOfWork.Invitations.Add(invitation);
        await unitOfWork.SaveChangesAsync(ct);

        // build the accept URL
        var acceptUrl = appUrlService.BuildInvitationUrl(rawToken);

        var inviter = await unitOfWork.Users.GetByIdAsync(currentUser.Id.Value, ct);
        var inviterName = inviter is not null
            ? $"{inviter.FullName}"
            : $"{TechFlowConstants.AppName}";

        // load company name for the email
        var company = await unitOfWork.Companies.GetByIdAsync(companyId, ct);
        var companyName = company?.Name ?? TechFlowConstants.AppName;

        // send the email (simulated in dev — prints to console)
        await emailService.SendInvitationAsync(
            toEmail: command.Email,
            invitedByName: inviterName,
            companyName: companyName,
            acceptUrl: acceptUrl,
            expiresAt: invitation.ExpiresAt,
            ct: ct);

        return invitation.ToDto();
    }
}