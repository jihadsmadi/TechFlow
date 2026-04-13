using TechFlow.Domain.Common;

namespace TechFlow.Domain.Invitations.Events;

// Fired when invitation is created
// Carries the RAW token — email service reads it from this event
// Raw token is never stored in DB, only passed through this event once
public sealed class InvitationCreatedEvent : DomainEvent
{
    public Guid InvitationId { get; }
    public Guid CompanyId { get; }
    public string Email { get; }
    public string RawToken { get; }   // ← send this in the email link
    public DateTimeOffset ExpiresAt { get; }

    public InvitationCreatedEvent(
        Guid invitationId,
        Guid companyId,
        string email,
        string rawToken,
        DateTimeOffset expiresAt)
    {
        InvitationId = invitationId;
        CompanyId = companyId;
        Email = email;
        RawToken = rawToken;
        ExpiresAt = expiresAt;
    }
}
