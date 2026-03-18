using TechFlow.Application.Features.Tasks.Dtos;

namespace TechFlow.Application.Features.Sprints.Dtos;

public sealed record SprintDto(
    Guid Id,
    Guid ProjectId,
    Guid CompanyId,
    Guid CreatedByUserId,
    int SprintNumber,
    string DisplayName,
    string? Name,
    string? Goal,
    string Status,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    DateTimeOffset? ActualEndDate,
    bool IsLocked,
    IReadOnlyList<TaskSummaryDto> Tasks,
    int TotalTasks,
    int CompletedTasks,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record SprintSummaryDto(
    Guid Id,
    Guid ProjectId,
    int SprintNumber,
    string DisplayName,
    string? Name,
    string? Goal,
    string Status,
    DateTimeOffset StartDate,
    DateTimeOffset EndDate,
    DateTimeOffset? ActualEndDate,
    bool IsLocked,
    int TotalTasks,
    int CompletedTasks);
