using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Users;

/// <summary>
/// Owned entity — part of the User aggregate.
/// Mapped to USER_PREFERENCES table via EF Core owned entity configuration.
/// Created automatically with defaults when a User is registered.
/// </summary>
public sealed class UserPreferences
{
    // Notification toggles
    public bool NotifyOnTaskAssigned { get; private set; } = true;
    public bool NotifyOnCommentAdded { get; private set; } = true;
    public bool NotifyOnDueDateNear { get; private set; } = true;
    public bool NotifyOnTaskMoved { get; private set; } = false;
    public bool NotifyOnMentioned { get; private set; } = true;
    public int DueDateReminderDays { get; private set; } = 1;

    // UI preferences
    public string Theme { get; private set; } = "System";
    public string BoardView { get; private set; } = "Board";
    public string Language { get; private set; } = "en";

    private static readonly string[] ValidThemes = ["Light", "Dark", "System"];
    private static readonly string[] ValidBoardViews = ["Board", "List"];
    private static readonly string[] ValidLanguages = ["en", "ar", "fr"];

    private UserPreferences() { }

    internal static UserPreferences CreateDefault() => new();

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> UpdateNotifications(
        bool notifyOnTaskAssigned,
        bool notifyOnCommentAdded,
        bool notifyOnDueDateNear,
        bool notifyOnTaskMoved,
        bool notifyOnMentioned,
        int dueDateReminderDays)
    {
        if (!IsValidReminderDays(dueDateReminderDays))
            return UserErrors.InvalidReminderDays;

        NotifyOnTaskAssigned = notifyOnTaskAssigned;
        NotifyOnCommentAdded = notifyOnCommentAdded;
        NotifyOnDueDateNear = notifyOnDueDateNear;
        NotifyOnTaskMoved = notifyOnTaskMoved;
        NotifyOnMentioned = notifyOnMentioned;
        DueDateReminderDays = dueDateReminderDays;

        return Result.Updated;
    }

    public Result<Updated> UpdateUiPreferences(string theme, string boardView, string language)
    {
        if (!IsValidTheme(theme))
            return UserErrors.InvalidTheme(theme);

        if (!IsValidBoardView(boardView))
            return UserErrors.InvalidBoardView(boardView);

        if (!IsValidLanguage(language))
            return UserErrors.InvalidLanguage(language);

        Theme = theme;
        BoardView = boardView;
        Language = language;

        return Result.Updated;
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidReminderDays(int days) => days >= 1 && days <= 30;
    private static bool IsValidTheme(string theme) => ValidThemes.Contains(theme);
    private static bool IsValidBoardView(string boardView) => ValidBoardViews.Contains(boardView);
    private static bool IsValidLanguage(string language) => ValidLanguages.Contains(language);
}