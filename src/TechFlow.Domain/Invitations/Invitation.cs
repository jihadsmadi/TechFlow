using System.Security.Cryptography;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Invitations.Events;

namespace TechFlow.Domain.Invitations;

public sealed class Invitation : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid InvitedByUserId { get; private set; }
    public Guid RoleId { get; private set; }

    public string Email { get; private set; } = string.Empty;
    public Guid? ProjectId { get; private set; }      
    public InvitationType Type { get; private set; }

    public string TokenHash { get; private set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public DateTimeOffset? UsedAt { get; private set; }
    public bool IsRevoked { get; private set; }

    private Invitation() { }
    private Invitation(
          Guid id,
          Guid companyId,
          Guid invitedByUserId,
          Guid roleId,
          string email,
          Guid? projectId,
          InvitationType type,
          string tokenHash,
          DateTimeOffset expiresAt)
          : base(id)
    {
        CompanyId = companyId;
        InvitedByUserId = invitedByUserId;
        RoleId = roleId;
        Email = email;
        ProjectId = projectId;
        Type = type;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        IsUsed = false;
        IsRevoked = false;
    }
    public static Result<(Invitation Invitation, string RawToken)> Create(
       Guid companyId,
       Guid invitedByUserId,
       Guid roleId,
       string email,
       Guid? projectId = null)
    {
        if (companyId == Guid.Empty)
            return InvitationErrors.CompanyIdRequired;

        if (invitedByUserId == Guid.Empty)
            return InvitationErrors.InvitedByUserIdRequired;

        if (roleId == Guid.Empty)
            return InvitationErrors.RoleIdRequired;

        if (string.IsNullOrWhiteSpace(email))
            return InvitationErrors.EmailRequired;

        // Generate cryptographically secure token
        var rawTokenBytes = RandomNumberGenerator.GetBytes(64);
        var rawToken = Convert.ToBase64String(rawTokenBytes)
                                   .Replace('+', '-')
                                   .Replace('/', '_')
                                   .TrimEnd('=');

        var tokenHash = HashToken(rawToken);

        var type = projectId.HasValue
            ? InvitationType.Project
            : InvitationType.Company;

        var invitation = new Invitation(
            id: Guid.NewGuid(),
            companyId: companyId,
            invitedByUserId: invitedByUserId,
            roleId: roleId,
            email: email.Trim().ToLowerInvariant(),
            projectId: projectId,
            type: type,
            tokenHash: tokenHash,
            expiresAt: DateTimeOffset.UtcNow.AddDays(7));

        invitation.AddDomainEvent(new InvitationCreatedEvent(
            invitation.Id,
            invitation.CompanyId,
            invitation.Email,
            rawToken,
            invitation.ExpiresAt));

        return (invitation, rawToken);
    }

    public Result<Updated> MarkAsUsed(Guid userId)
    {
        if (IsRevoked)
            return InvitationErrors.Revoked;

        if (IsExpired())
            return InvitationErrors.Expired;

        if (IsUsed)
            return InvitationErrors.AlreadyUsed;

        IsUsed = true;
        UsedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new InvitationAcceptedEvent(Id,CompanyId,userId,Email)); 
        return Result.Updated;
    }

    public Result<Updated> Revoke()
    {
        if (IsUsed)
            return InvitationErrors.AlreadyUsed;

        if (IsRevoked)
            return InvitationErrors.AlreadyRevoked;

        IsRevoked = true;

        return Result.Updated;
    }

    // ── Helpers
    public bool IsExpired() => DateTimeOffset.UtcNow > ExpiresAt;

    public bool IsValid() => !IsUsed && !IsRevoked && !IsExpired();

   
    public static string HashToken(string rawToken)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(rawToken);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}