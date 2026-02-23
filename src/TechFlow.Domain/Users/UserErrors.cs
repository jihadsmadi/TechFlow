using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users;

public static class UserErrors
{
    // Identity bridge
    public static readonly Error IdentityIdRequired =
        Error.Validation("User.IdentityIdRequired", "Identity user ID is required.");

    // Company
    public static readonly Error CompanyIdRequired =
        Error.Validation("User.CompanyIdRequired", "Company ID is required.");

    // Name
    public static readonly Error FirstNameRequired =
        Error.Validation("User.FirstNameRequired", "First name is required.");

    public static readonly Error FirstNameTooLong =
        Error.Validation("User.FirstNameTooLong", "First name is too long.");

    public static readonly Error LastNameRequired =
        Error.Validation("User.LastNameRequired", "Last name is required.");

    public static readonly Error LastNameTooLong =
        Error.Validation("User.LastNameTooLong", "Last name is too long.");

    // Email
    public static readonly Error EmailRequired =
        Error.Validation("User.EmailRequired", "Email is required.");

    public static readonly Error EmailInvalid =
        Error.Validation("User.EmailInvalid", "Email is not valid.");

    public static readonly Error EmailAlreadyExists =
        Error.Conflict("User.EmailAlreadyExists", "A user with this email already exists.");

    // State
    public static readonly Error NotFound =
        Error.NotFound("User.NotFound", "User was not found.");

    public static readonly Error AlreadyInactive =
        Error.Conflict("User.AlreadyInactive", "User is already inactive.");

    public static readonly Error AlreadyActive =
        Error.Conflict("User.AlreadyActive", "User is already active.");

    // Preferences
    public static readonly Error InvalidReminderDays =
        Error.Validation("User.InvalidReminderDays", "Due date reminder must be between 1 and 30 days.");

    public static Error InvalidTheme(string theme) =>
        Error.Validation("User.InvalidTheme", $"'{theme}' is not valid. Valid themes: Light, Dark, System.");

    public static Error InvalidBoardView(string view) =>
        Error.Validation("User.InvalidBoardView", $"'{view}' is not valid. Valid views: Board, List.");

    public static Error InvalidLanguage(string language) =>
        Error.Validation("User.InvalidLanguage", $"'{language}' is not supported. Supported: en, ar, fr.");
}