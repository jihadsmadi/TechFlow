using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Invitations;

namespace TechFlow.Application.Features.Invitaions.Commands.RevokeInvitaion;

public sealed class RevokeInvitationCommandHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<RevokeInvitationCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(
        RevokeInvitationCommand command,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var invitation = await unitOfWork.Invitations.GetByIdAsync(command.InvitationId, ct);
        if (invitation is null || invitation.CompanyId != currentUser.CompanyId)
            return InvitationErrors.NotFound;

        var result = invitation.Revoke();
        if (result.IsFailure)
            return result.TopError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Updated;
    }
}
