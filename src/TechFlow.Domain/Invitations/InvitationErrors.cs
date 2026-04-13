using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Invitations;

public static class InvitationErrors
{
    public static readonly Error CompanyIdRequired =
        Error.Validation("Invitation.CompanyIdRequired", "Company ID is required.");

    public static readonly Error InvitedByUserIdRequired =
        Error.Validation("Invitation.InvitedByUserIdRequired", "Inviting user ID is required.");

    public static readonly Error RoleIdRequired =
        Error.Validation("Invitation.RoleIdRequired", "Role ID is required.");

    public static readonly Error EmailRequired =
        Error.Validation("Invitation.EmailRequired", "Email address is required.");

    public static readonly Error NotFound =
        Error.NotFound("Invitation.NotFound", "Invitation not found or has expired.");

    public static readonly Error Expired =
        Error.Conflict("Invitation.Expired", "This invitation has expired.");

    public static readonly Error AlreadyUsed =
        Error.Conflict("Invitation.AlreadyUsed", "This invitation has already been used.");

    public static readonly Error Revoked =
        Error.Conflict("Invitation.Revoked", "This invitation has been revoked.");

    public static readonly Error AlreadyRevoked =
        Error.Conflict("Invitation.AlreadyRevoked", "This invitation is already revoked.");

    public static readonly Error PendingInviteExists =
        Error.Conflict("Invitation.PendingInviteExists", "A pending invitation already exists for this email.");

    public static readonly Error UserAlreadyMember =
        Error.Conflict("Invitation.UserAlreadyMember", "This user is already a member of the company.");

    public static readonly Error PasswordRequired =
        Error.Validation("Invitation.PasswordRequired", "Password is required.");

    public static readonly Error PasswordTooShort =
        Error.Validation("Invitation.PasswordTooShort", "Password must be at least 8 characters.");
}