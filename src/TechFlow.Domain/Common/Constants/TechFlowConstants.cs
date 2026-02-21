
namespace TechFlow.Domain.Common.Constants;

public static class TechFlowConstants
{
    public const string AppName = "TechFlow";
    public const string DatabaseConnection = "DefaultConnection";

    public static class Roles
    {
        public const string Admin = "Admin";
        public const string ProjectManager = "ProjectManager";
        public const string Developer = "Developer";
        public const string Intern = "Intern";
    }

    public static class Validation
    {
        public const int MaxNameLength = 100;
        public const int MaxDescriptionLength = 2000;
        public const int MinPasswordLength = 6;
    }
}