using TechFlow.Application.Features.Tasks.Dtos;
using TechFlow.Domain.Tasks.Subtasks;

namespace TechFlow.Application.Features.Tasks.Mappers;

public static class SubtaskMapper
{
    public static SubtaskDto ToDto(this Subtask entity) => new(
        Id: entity.Id,
        TaskId: entity.TaskId,
        Title: entity.Title,
        IsCompleted: entity.IsCompleted,
        CreatedAt: entity.CreatedAt
    );

    public static List<SubtaskDto> ToDtos(this IEnumerable<Subtask> entities) =>
        [.. entities.Select(e => e.ToDto())];
}