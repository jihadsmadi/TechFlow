using System.Net.Mail;
using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Users.Events;
using TechFlow.Domain.Users.UserCompanyRoles;
using TechFlow.Domain.Users.UserProjectRoles;

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

    // Navigation
    private readonly List<UserCompanyRole> _userCompanyRoles = [];
    public IReadOnlyList<UserCompanyRole> UserCompanyRoles => _userCompanyRoles.AsReadOnly();

    private readonly List<UserProjectRole> _userProjectRoles = [];
    public IReadOnlyList<UserProjectRole> UserProjectRoles => _userProjectRoles.AsReadOnly();

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

    // ── Factory 

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

    // ── Business 

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

    public Result<Updated> AssignCompanyRole(UserCompanyRole userCompanyRole)
    {
        var alreadyAssigned = _userCompanyRoles.Any(ur =>
            ur.RoleId == userCompanyRole.RoleId);

        if (alreadyAssigned)
            return UserCompanyRoleErrors.AlreadyAssigned;

        _userCompanyRoles.Add(userCompanyRole);

        AddDomainEvent(new UserCompanyRoleAssignedEvent(Id, userCompanyRole.RoleId));

        return Result.Updated;
    }

    public Result<Updated> RemoveCompanyRole(Guid roleId)
    {
        var existing = _userCompanyRoles.FirstOrDefault(ur =>
            ur.RoleId == roleId);

        if (existing is null)
            return UserCompanyRoleErrors.NotFound;

        _userCompanyRoles.Remove(existing);

        return Result.Updated;
    }
    public Result<Updated> AssignProjectRole(UserProjectRole userProjectRole)
    {
        var alreadyAssigned = _userProjectRoles.Any(ur =>
            ur.RoleId == userProjectRole.RoleId);

        if (alreadyAssigned)
            return UserProjectRoleErrors.AlreadyAssigned;

        _userProjectRoles.Add(userProjectRole);

        AddDomainEvent(new UserProjectRoleAssignedEvent(Id,userProjectRole.ProjectId, userProjectRole.RoleId));

        return Result.Updated;
    }

    public Result<Updated> RemoveProjectRole(Guid roleId)
    {
        var existing = _userProjectRoles.FirstOrDefault(ur =>
            ur.RoleId == roleId);

        if (existing is null)
            return UserProjectRoleErrors.NotFound;

        _userProjectRoles.Remove(existing);

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
        _userProjectRoles.Any(ur =>
            ur.RoleId == roleId && ur.ProjectId == projectId);

    // ── Private Validation 

    private static bool IsValidId(Guid id) => id != Guid.Empty;

    private static bool IsValidEmail(string email)
    {
        try { _ = new MailAddress(email); return true; }
        catch { return false; }
    }
}