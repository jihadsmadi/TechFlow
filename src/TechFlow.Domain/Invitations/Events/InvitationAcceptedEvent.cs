using TechFlow.Domain.Common;

namespace TechFlow.Domain.Invitations.Events;

// Fired when invitation is accepted
public sealed class InvitationAcceptedEvent : DomainEvent
{
    public Guid InvitationId { get; }
    public Guid CompanyId { get; }
    public Guid UserId { get; }
    public string Email { get; }

    public InvitationAcceptedEvent(
        Guid invitationId,
        Guid companyId,
        Guid userId,
        string email)
    {
        InvitationId = invitationId;
        CompanyId = companyId;
        UserId = userId;
        Email = email;
    }
}