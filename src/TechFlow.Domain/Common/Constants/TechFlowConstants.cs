
namespace TechFlow.Domain.Common.Constants;

public static class TechFlowConstants
{
    public const string AppName = "TechFlow";
    public const string DatabaseConnection = "DefaultConnection";

    public static class Cashe
    {
        public const int PermissionExpirTimeSpan = 12; // in hours
        public const int RoleExpirTimeSpan = 24;

        public const int CompanyExpirationHours = 1;
        public const int PermissionExpirationHours = 24;
        public const int RoleExpirationHours = 24;
    }
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string ProjectManager = "ProjectManager";
        public const string Developer = "Developer";
        public const string Intern = "Intern";
    }

    public static class Validation
    {
        public const int MaxNameLength = 50;
        public const int MaxDescriptionLength = 2000;
        public const int MinPasswordLength = 6;
    }
}