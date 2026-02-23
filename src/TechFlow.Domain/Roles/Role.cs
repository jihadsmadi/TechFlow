using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;

namespace TechFlow.Domain.Roles;

public sealed class Role : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsSystemRole { get; private set; }

    private readonly List<Permission> _permissions = [];
    public IReadOnlyList<Permission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    private Role(Guid id, string name, string description, bool isSystemRole)
        : base(id)
    {
        Name = name;
        Description = description;
        IsSystemRole = isSystemRole;
    }


    public static Result<Role> Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return RoleErrors.NameTooLong;

        return new Role(
            id: Guid.NewGuid(),
            name: name.Trim(),
            description: description.Trim(),
            isSystemRole: false
        );
    }

    /// <summary>
    /// Used only by the seeder to create protected system roles.
    /// </summary>
    internal static Role CreateSystem(Guid id, string name, string description)
    {
        return new Role(id, name, description, isSystemRole: true);
    }


    public Result<Updated> Update(string name, string description)
    {
        if (IsSystemRole)
            return RoleErrors.CannotModifySystemRole;

        if (string.IsNullOrWhiteSpace(name))
            return RoleErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return RoleErrors.NameTooLong;

        Name = name.Trim();
        Description = description.Trim();
        return Result.Updated;
    }

    public Result<Updated> GrantPermission(Permission permission)
    {
        if (HasPermission(permission.Name))
            return RoleErrors.PermissionAlreadyGranted(permission.Name);

        _permissions.Add(permission);
        return Result.Updated;
    }

    public Result<Updated> RevokePermission(Permission permission)
    {
        if (IsSystemRole)
            return RoleErrors.CannotModifySystemRole;

        var existing = _permissions.FirstOrDefault(p => p.Id == permission.Id);

        if (existing is null)
            return RoleErrors.PermissionNotGranted(permission.Name);

        _permissions.Remove(existing);
        return Result.Updated;
    }

    public bool HasPermission(string permissionName) =>
        _permissions.Any(p => p.Name == permissionName);


    public Result<Deleted> Delete()
    {
        if (IsSystemRole)
            return RoleErrors.CannotDeleteSystemRole;

        return Result.Deleted;
    }
}