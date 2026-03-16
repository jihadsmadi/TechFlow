using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.Subtasks;

public sealed class Subtask : Entity
{
    public Guid TaskId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public bool IsCompleted { get; private set; } = false;
    public DateTimeOffset CreatedAt { get; private set; }

    private Subtask() { }

    private Subtask(Guid id, Guid taskId, Guid createdByUserId, string title)
        : base(id)
    {
        TaskId = taskId;
        CreatedByUserId = createdByUserId;
        Title = title;
        IsCompleted = false;
        CreatedAt = DateTimeOffset.UtcNow;
    }

    internal static Result<Subtask> Create(Guid taskId, Guid createdByUserId, string title)
    {
        if (!IsValidId(taskId))
            return SubtaskErrors.TaskIdRequired;

        if (!IsValidId(createdByUserId))
            return SubtaskErrors.CreatedByRequired;

        if (string.IsNullOrWhiteSpace(title))
            return SubtaskErrors.TitleRequired;

        if (title.Length > TechFlowConstants.Validation.MaxNameLength)
            return SubtaskErrors.TitleTooLong;

        return new Subtask(Guid.NewGuid(), taskId, createdByUserId, title.Trim());
    }


    public Result<Updated> Complete()
    {
        if (IsCompleted)
            return SubtaskErrors.AlreadyCompleted;

        IsCompleted = true;
        return Result.Updated;
    }

    public Result<Updated> Reopen()
    {
        if (!IsCompleted)
            return SubtaskErrors.NotCompleted;

        IsCompleted = false;
        return Result.Updated;
    }

    public Result<Updated> Rename(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            return SubtaskErrors.TitleRequired;

        if (title.Length > TechFlowConstants.Validation.MaxNameLength)
            return SubtaskErrors.TitleTooLong;

        Title = title.Trim();
        return Result.Updated;
    }


    private static bool IsValidId(Guid id) => id != Guid.Empty;
}