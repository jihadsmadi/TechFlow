using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Sprints.ValueObjects;

public sealed class SprintItem : Entity
{
    public Guid SprintId { get; private set; }
    public Guid TaskId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }

    private SprintItem() { }

    private SprintItem(Guid id, Guid sprintId, Guid taskId) : base(id)
    {
        SprintId = sprintId;
        TaskId = taskId;
        AddedAt = DateTimeOffset.UtcNow;
    }

    internal static Result<SprintItem> Create(Guid sprintId, Guid taskId)
    {
        if (sprintId == Guid.Empty)
            return SprintErrors.SprintIdRequired;

        if (taskId == Guid.Empty)
            return SprintErrors.TaskIdRequired;

        return new SprintItem(Guid.NewGuid(), sprintId, taskId);
    }
}
