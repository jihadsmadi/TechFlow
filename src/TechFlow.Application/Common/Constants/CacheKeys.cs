namespace TechFlow.Application.Common.Constants;

public static class CacheKeys
{
    public static IReadOnlyCollection<string> AllTags => new[]
    {
        Permissions.Tag,
        Roles.Tag,
        Companies.Tag
    };
    public static class Permissions
    {
        public const string All = "permissions:all";
        public static string ById(Guid id) => $"permissions:{id}";
        public static string ByGroup(string group) => $"permissions:group:{group}";
        public const string Tag = "permissions";
        public const int ExpirationHours = 24;
    }
    public static class Roles
    {
        public const string All = "roles:all";
        public static string ById(Guid id) => $"roles:{id}";
        public const string Tag = "roles";
        public const int ExpirationHours = 24;
    }
    public static class Companies
    {
        public const string All = "companies:all";
        public static string ById(Guid id) => $"companies:{id}";
        public static string BySlug(string slug) => $"companies:slug:{slug}";
        public const string Tag = "companies";
        public const int ExpirationHours = 1;
    }
}