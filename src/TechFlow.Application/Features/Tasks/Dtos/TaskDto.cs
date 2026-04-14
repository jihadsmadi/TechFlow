namespace TechFlow.Application.Features.Tasks.Dtos;

public sealed record TaskDto(
    Guid Id,
    Guid ListId,
    Guid ProjectId,
    Guid CompanyId,
    Guid CreatedByUserId,
    string Title,
    string? Description,
    string Priority,
    string Type,
    double DisplayOrder,
    DateTimeOffset? DueDate,
    int? EstimatedMinutes,
    int? ActualMinutes,
    bool IsCompleted,
    DateTimeOffset? CompletedAt,
    IReadOnlyList<SubtaskDto> Subtasks,
    IReadOnlyList<TaskAssignmentDto> Assignments,
    int SubtasksTotal,
    int SubtasksCompleted,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);

public sealed record TaskSummaryDto(
    Guid Id,
    Guid ListId,
    string Title,
    string Priority,
    string Type,
    double DisplayOrder,
    DateTimeOffset? DueDate,
    bool IsCompleted,
    int SubtasksTotal,
    int SubtasksCompleted,
    IReadOnlyList<Guid> AssignedUserIds);

public sealed record TaskAssignmentDto(
    Guid Id,
    Guid TaskId,
    Guid UserId,
    string UserName,        
    string UserEmail,       
    string? UserAvatarUrl,  
    Guid AssignedByUserId,
    DateTimeOffset AssignedAt);
public sealed record SubtaskDto(
    Guid Id,
    Guid TaskId,
    string Title,
    bool IsCompleted,
    DateTimeOffset CreatedAt);
public sealed record MyTaskDto(
    Guid Id,
    Guid ProjectId,
    Guid ListId,
    string Title,
    string Priority,
    string Type,
    double DisplayOrder,
    DateTimeOffset? DueDate,
    bool IsCompleted,
    int SubtasksTotal,
    int SubtasksCompleted,
    IReadOnlyList<Guid> AssignedUserIds);