using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Projects;

public static class ProjectErrors
{
    // IDs
    public static readonly Error CompanyIdRequired =
        Error.Validation("Project.CompanyIdRequired", "Company ID is required.");

    public static readonly Error CreatedByRequired =
        Error.Validation("Project.CreatedByRequired", "Created by user ID is required.");

    // Name
    public static readonly Error NameRequired =
        Error.Validation("Project.NameRequired", "Project name is required.");

    public static readonly Error NameTooLong =
        Error.Validation("Project.NameTooLong", "Project name is too long.");

    // Status
    public static Error InvalidStatus(string status) =>
        Error.Validation("Project.InvalidStatus",
            $"'{status}' is not valid. Valid statuses: Active, OnHold, Completed, Archived.");

    // Dates
    public static readonly Error InvalidDateRange =
        Error.Validation("Project.InvalidDateRange", "Start date must be before end date.");

    // Archive
    public static readonly Error AlreadyArchived =
        Error.Conflict("Project.AlreadyArchived", "Project is already archived.");

    public static readonly Error NotArchived =
        Error.Conflict("Project.NotArchived", "Project is not archived.");

    public static readonly Error CannotModifyArchived =
        Error.Forbidden("Project.CannotModifyArchived", "Cannot modify an archived project.");

    // Members
    public static readonly Error MemberAlreadyExists =
        Error.Conflict("Project.MemberAlreadyExists", "User is already a member of this project.");

    public static readonly Error MemberNotFound =
        Error.NotFound("Project.MemberNotFound", "User is not a member of this project.");

    public static readonly Error CannotRemoveOwner =
        Error.Forbidden("Project.CannotRemoveOwner", "The project owner cannot be removed from the project.");

    // General
    public static readonly Error NotFound =
        Error.NotFound("Project.NotFound", "Project was not found.");
}
