using TechFlow.Application.Features.Sprints.Dtos;
using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Sprints;

namespace TechFlow.Application.Features.Sprints.Mappers;

public static class SprintMapper
{

    public static SprintDto ToDto(
        this Sprint sprint,
        IReadOnlyList<TaskSummaryDto> tasks) => new(
            Id: sprint.Id,
            ProjectId: sprint.ProjectId,
            CompanyId: sprint.CompanyId,
            CreatedByUserId: sprint.CreatedByUserId,
            SprintNumber: sprint.SprintNumber,
            DisplayName: sprint.DisplayName,
            Name: sprint.Name,
            Goal: sprint.Goal,
            Status: sprint.Status,
            StartDate: sprint.StartDate,
            EndDate: sprint.EndDate,
            ActualEndDate: sprint.ActualEndDate,
            IsLocked: sprint.IsLocked,
            Tasks: tasks,
            TotalTasks: tasks.Count,
            CompletedTasks: tasks.Count(t => t.IsCompleted),
            CreatedAt: sprint.CreatedAtUtc,
            UpdatedAt: sprint.LastModifiedUtc);

    public static SprintSummaryDto ToSummaryDto(
        this Sprint sprint,
        int totalTasks = 0,
        int completedTasks = 0) => new(
            Id: sprint.Id,
            ProjectId: sprint.ProjectId,
            SprintNumber: sprint.SprintNumber,
            DisplayName: sprint.DisplayName,
            Name: sprint.Name,
            Goal: sprint.Goal,
            Status: sprint.Status,
            StartDate: sprint.StartDate,
            EndDate: sprint.EndDate,
            ActualEndDate: sprint.ActualEndDate,
            IsLocked: sprint.IsLocked,
            TotalTasks: totalTasks,
            CompletedTasks: completedTasks);

    public static List<SprintSummaryDto> ToSummaryDtos(
        this IEnumerable<Sprint> sprints,
        Dictionary<Guid, (int Total, int Completed)> counts) =>
        [.. sprints.Select(s =>
        {
            counts.TryGetValue(s.Id, out var c);
            return s.ToSummaryDto(c.Total, c.Completed);
        })];
}