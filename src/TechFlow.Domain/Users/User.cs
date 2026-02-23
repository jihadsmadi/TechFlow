using System.Net.Mail;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Users.Events;
using TechFlow.Domain.Users.UserRoles;

namespace TechFlow.Domain.Users;

public sealed class User : AuditableEntity
{
    public Guid IdentityUserId { get; private set; }  // bridge to AspNetUsers
    public Guid CompanyId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? AvatarUrl { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    // Owned entity — auto created on registration
    public UserPreferences Preferences { get; private set; } = null!;

    // Navigatio
    private readonly List<UserRole> _userRoles = [];
    public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();

    private User() { }

    private User(Guid id, Guid identityUserId, Guid companyId, string firstName, string lastName, string email)
        : base(id)
    {
        IdentityUserId = identityUserId;
        CompanyId = companyId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Preferences = UserPreferences.CreateDefault();
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<User> Create(
        Guid identityUserId,
        Guid companyId,
        string firstName,
        string lastName,
        string email)
    {
        if (!IsValidId(identityUserId))
            return UserErrors.IdentityIdRequired;

        if (!IsValidId(companyId))
            return UserErrors.CompanyIdRequired;

        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired;

        if (firstName.Length > TechFlowConstants.Validation.MaxNameLength)
            return UserErrors.FirstNameTooLong;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired;

        if (lastName.Length > TechFlowConstants.Validation.MaxNameLength)
            return UserErrors.LastNameTooLong;

        if (string.IsNullOrWhiteSpace(email))
            return UserErrors.EmailRequired;

        if (!IsValidEmail(email))
            return UserErrors.EmailInvalid;

        var user = new User(
            id: Guid.NewGuid(),
            identityUserId: identityUserId,
            companyId: companyId,
            firstName: firstName.Trim(),
            lastName: lastName.Trim(),
            email: email.Trim().ToLower()
        );

        user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.CompanyId, user.Email));

        return user;
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Update(string firstName, string lastName, string? avatarUrl = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired;

        if (firstName.Length > TechFlowConstants.Validation.MaxNameLength)
            return UserErrors.FirstNameTooLong;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired;

        if (lastName.Length > TechFlowConstants.Validation.MaxNameLength)
            return UserErrors.LastNameTooLong;

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        AvatarUrl = avatarUrl?.Trim();

        return Result.Updated;
    }

    public Result<Updated> AssignRole(UserRole userRole)
    {
        var alreadyAssigned = _userRoles.Any(ur =>
            ur.RoleId == userRole.RoleId &&
            ur.ProjectId == userRole.ProjectId);

        if (alreadyAssigned)
            return UserRoleErrors.AlreadyAssigned;

        _userRoles.Add(userRole);

        AddDomainEvent(new UserRoleAssignedEvent(Id, userRole.RoleId, userRole.ProjectId));

        return Result.Updated;
    }

    public Result<Updated> RemoveRole(Guid roleId, Guid? projectId)
    {
        var existing = _userRoles.FirstOrDefault(ur =>
            ur.RoleId == roleId &&
            ur.ProjectId == projectId);

        if (existing is null)
            return UserRoleErrors.NotFound;

        _userRoles.Remove(existing);

        return Result.Updated;
    }

    public Result<Updated> Deactivate()
    {
        if (!IsActive)
            return UserErrors.AlreadyInactive;

        IsActive = false;
        AddDomainEvent(new UserDeactivatedEvent(Id, CompanyId));

        return Result.Updated;
    }

    public Result<Updated> Activate()
    {
        if (IsActive)
            return UserErrors.AlreadyActive;

        IsActive = true;
        AddDomainEvent(new UserActivatedEvent(Id));

        return Result.Updated;
    }

    public bool HasRoleInProject(Guid roleId, Guid projectId) =>
        _userRoles.Any(ur =>
            ur.RoleId == roleId &&
            (ur.IsCompanyWide || ur.ProjectId == projectId));

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;

    private static bool IsValidEmail(string email)
    {
        try { _ = new MailAddress(email); return true; }
        catch { return false; }
    }
}