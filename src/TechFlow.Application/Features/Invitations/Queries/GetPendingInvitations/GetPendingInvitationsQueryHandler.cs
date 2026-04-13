using MediatR;
using TechFlow.Application.Common.Errors;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Features.Invitaions.Queries.GetPendingInvitations;
using TechFlow.Application.Features.Invitations.Dtos;
using TechFlow.Application.Features.Invitations.Mappers;
using TechFlow.Domain.Common.Results;


namespace TechFlow.Application.Features.Invitations.Queries.GetPendingInvitations;

public sealed class GetPendingInvitationsQueryHandler(
    IUnitOfWork unitOfWork,
    IUser currentUser)
    : IRequestHandler<GetPendingInvitationsQuery, Result<IReadOnlyList<InvitationDto>>>
{
    public async Task<Result<IReadOnlyList<InvitationDto>>> Handle(
        GetPendingInvitationsQuery query,
        CancellationToken ct)
    {
        if (currentUser.Id is null)
            return ApplicationErrors.Unauthorized;

        var invitations = await unitOfWork.Invitations
            .GetPendingByCompanyAsync(currentUser.CompanyId, ct);

        return invitations.ToDtos();
    }
}