using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Tasks.TaskAssignments;

namespace TechFlow.Application.Features.Tasks.Mappers;

public static class TaskAssignmentMapper
{
    public static TaskAssignmentDto ToDto(
        this TaskAssignment entity,
        IReadOnlyDictionary<Guid, (string Name, string Email, string? AvatarUrl)>? userLookup = null)
    {
        var userInfo = userLookup?.GetValueOrDefault(entity.UserId);

        return new TaskAssignmentDto(
            Id: entity.Id,
            TaskId: entity.TaskId,
            UserId: entity.UserId,
            UserName: userInfo?.Name ?? entity.UserId.ToString(),
            UserEmail: userInfo?.Email ?? string.Empty,
            UserAvatarUrl: userInfo?.AvatarUrl,
            AssignedByUserId: entity.AssignedByUserId,
            AssignedAt: entity.AssignedAt
        );
    }

    public static List<TaskAssignmentDto> ToDtos(
        this IEnumerable<TaskAssignment> entities,
        IReadOnlyDictionary<Guid, (string Name, string Email, string? AvatarUrl)>? userLookup = null) =>
        [.. entities.Select(e => e.ToDto(userLookup))];
}