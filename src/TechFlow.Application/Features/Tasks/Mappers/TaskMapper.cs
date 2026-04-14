using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Tasks;
using TechFlow.Domain.Tasks.Subtasks;
using TechFlow.Domain.Tasks.TaskAssignments;

namespace TechFlow.Application.Features.Tasks.Mappers;

public static class TaskMapper
{
    // ── Full DTO (for GetById)
    public static TaskDto ToDto(
        this Domain.Tasks.Task entity,
        IReadOnlyDictionary<Guid, (string Name, string Email, string? AvatarUrl)>? userLookup = null)
    {
        var (completed, total) = entity.GetSubtaskProgress();

        var assignmentDtos = entity.Assignments.Select(a => a.ToDto(userLookup)).ToList();

        return new TaskDto(
            Id: entity.Id,
            ListId: entity.ListId,
            ProjectId: entity.ProjectId,
            CompanyId: entity.CompanyId,
            CreatedByUserId: entity.CreatedByUserId,
            Title: entity.Title,
            Description: entity.Description,
            Priority: entity.Priority,
            Type: entity.Type,
            DisplayOrder: entity.DisplayOrder,
            DueDate: entity.DueDate,
            EstimatedMinutes: entity.EstimatedMinutes,
            ActualMinutes: entity.ActualMinutes,
            IsCompleted: entity.IsCompleted,
            CompletedAt: entity.CompletedAt,
            Subtasks: entity.Subtasks.ToDtos(),
            Assignments: assignmentDtos,
            SubtasksTotal: total,
            SubtasksCompleted: completed,
            CreatedAt: entity.CreatedAtUtc,
            UpdatedAt: entity.LastModifiedUtc
        );
    }

    // ── Summary DTO (lightweight, for board views)
    public static TaskSummaryDto ToSummaryDto(this Domain.Tasks.Task entity)
    {
        var (completed, total) = entity.GetSubtaskProgress();

        return new TaskSummaryDto(
            Id: entity.Id,
            ListId: entity.ListId,
            Title: entity.Title,
            Priority: entity.Priority,
            Type: entity.Type,
            DisplayOrder: entity.DisplayOrder,
            DueDate: entity.DueDate,
            IsCompleted: entity.IsCompleted,
            SubtasksTotal: total,
            SubtasksCompleted: completed,
            AssignedUserIds: entity.Assignments.Select(a => a.UserId).ToList()
        );
    }

    // ── My Tasks DTO (cross-project view)
    public static MyTaskDto ToMyTaskDto(this Domain.Tasks.Task entity)
    {
        var (completed, total) = entity.GetSubtaskProgress();

        return new MyTaskDto(
            Id: entity.Id,
            ProjectId: entity.ProjectId,
            ListId: entity.ListId,
            Title: entity.Title,
            Priority: entity.Priority,
            Type: entity.Type,
            DisplayOrder: entity.DisplayOrder,
            DueDate: entity.DueDate,
            IsCompleted: entity.IsCompleted,
            SubtasksTotal: total,
            SubtasksCompleted: completed,
            AssignedUserIds: entity.Assignments.Select(a => a.UserId).ToList()
        );
    }

    // ── Bulk mappings
    public static List<TaskSummaryDto> ToSummaryDtos(this IEnumerable<Domain.Tasks.Task> entities) =>
        [.. entities.Select(e => e.ToSummaryDto())];

    public static List<MyTaskDto> ToMyTaskDtos(this IEnumerable<Domain.Tasks.Task> entities) =>
        [.. entities.Select(e => e.ToMyTaskDto())];
}