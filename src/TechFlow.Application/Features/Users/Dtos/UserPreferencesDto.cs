namespace TechFlow.Application.Features.Users.Dtos;

public sealed record UserPreferencesDto(
    string Theme,
    string BoardView,
    string Language,
    bool NotifyOnTaskAssigned,
    bool NotifyOnCommentAdded,
    bool NotifyOnDueDateNear,
    bool NotifyOnTaskMoved,
    bool NotifyOnMentioned,
    int DueDateReminderDays);