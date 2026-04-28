namespace TechFlow.Application.Features.Projects.Dtos;

public sealed record ProjectDto(
    Guid Id,
    Guid CompanyId,
    Guid CreatedByUserId,
    string Name,
    string? Description,
    string Status,
    string Color,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate,
    bool IsArchived,
    DateTimeOffset? ArchivedAt,
    int MemberCount,
    ProjectSettingsDto Settings,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record ProjectSummaryDto(
    Guid Id,
    string Name,
    string? Description,
    string Status,
    string Color,
    bool IsArchived,
    int MemberCount,
    DateTimeOffset? StartDate,
    DateTimeOffset? EndDate);

public sealed record ProjectMemberDto(
    Guid Id,
    Guid UserId,
    Guid ProjectId,
    Guid AddedByUserId,
    DateTimeOffset AddedAt);

public sealed record ProjectSettingsDto(
    List<string> DefaultListNames,
    string DefaultTaskType,
    string DefaultPriority,
    bool AutoAssignCreator,
    bool RequireEstimate,
    bool AllowSubtasks,
    bool SprintLockOnStart,
    int SprintDurationDays,
    string IncompleteTasksAction);
