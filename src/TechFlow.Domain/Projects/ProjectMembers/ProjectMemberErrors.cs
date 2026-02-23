using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Projects.ProjectMembers;

public static class ProjectMemberErrors
{
    public static readonly Error ProjectIdRequired =
        Error.Validation("ProjectMember.ProjectIdRequired", "Project ID is required.");

    public static readonly Error UserIdRequired =
        Error.Validation("ProjectMember.UserIdRequired", "User ID is required.");

    public static readonly Error AddedByRequired =
        Error.Validation("ProjectMember.AddedByRequired", "Added by user ID is required.");
}