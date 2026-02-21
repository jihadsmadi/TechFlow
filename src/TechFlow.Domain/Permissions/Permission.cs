using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Permissions.Const;

namespace TechFlow.Domain.Permissions
{
    public sealed class Permission : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string Group { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;

        private Permission() { }

        private Permission(Guid id, string name, string group, string description)
            : base(id)
        {
            Name = name;
            Group = group;
            Description = description;
        }

        public static Result<Permission> Create(string name, string group, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                return PermissionErrors.NameRequired;

            if (string.IsNullOrWhiteSpace(group))
                return PermissionErrors.GroupRequired;

            if (!IsValidGroup(group))
                return PermissionErrors.InvalidGroup(group);

            if (!IsValidNameFormat(name))
                return PermissionErrors.InvalidNameFormat;

            return new Permission(
                id: Guid.NewGuid(),
                name: name.Trim().ToLower(),
                group: group.Trim(),
                description: description.Trim()
            );
        }

        public Result<Updated> Update(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return PermissionErrors.DescriptionRequired;

            Description = description.Trim();
            return Result.Updated;
        }
        private static bool IsValidNameFormat(string name) =>
        name.Contains('.') &&
        name.IndexOf('.') > 0 &&
        name.IndexOf('.') < name.Length - 1;

        private static bool IsValidGroup(string group) =>
            PermissionGroups.All.Contains(group);
    }
}
