using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.Domain.Permissions;

public static class PermissionErrors
{
    public static readonly Error NameRequired =
        Error.Validation("Permission.NameRequired", "Permission name is required.");

    public static readonly Error GroupRequired =
        Error.Validation("Permission.GroupRequired", "Permission group is required.");

    public static readonly Error DescriptionRequired =
        Error.Validation("Permission.DescriptionRequired", "Description is required.");

    public static readonly Error InvalidNameFormat =
        Error.Validation("Permission.InvalidNameFormat", "Permission name must follow the format 'group.action' (e.g. tasks.create).");

    public static Error InvalidGroup(string group) =>
        Error.Validation("Permission.InvalidGroup", $"'{group}' is not a valid permission group. Valid groups: {string.Join(", ", PermissionGroups.All)}.");

    public static readonly Error NotFound =
        Error.NotFound("Permission.NotFound", "Permission was not found.");

    public static readonly Error AlreadyExists =
        Error.Conflict("Permission.AlreadyExists", "A permission with this name already exists.");
}