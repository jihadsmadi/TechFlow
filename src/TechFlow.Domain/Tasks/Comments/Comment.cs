using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Tasks.Comments;

namespace TechFlow.Domain.Tasks;

public sealed class Comment : Entity
{
    public Guid TaskId { get; private set; }
    public Guid UserId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsEdited { get; private set; } = false;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Comment() { }

    private Comment(Guid id, Guid taskId, Guid userId, string content)
        : base(id)
    {
        TaskId = taskId;
        UserId = userId;
        Content = content;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    internal static Result<Comment> Create(Guid taskId, Guid userId, string content)
    {
        if (!IsValidId(taskId))
            return CommentErrors.TaskIdRequired;

        if (!IsValidId(userId))
            return CommentErrors.UserIdRequired;

        if (string.IsNullOrWhiteSpace(content))
            return CommentErrors.ContentRequired;

        if (content.Length > TechFlowConstants.Validation.MaxDescriptionLength)
            return CommentErrors.ContentTooLong;

        return new Comment(Guid.NewGuid(), taskId, userId, content.Trim());
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Edit(string content, Guid requestedByUserId)
    {
        if (!IsOwner(requestedByUserId))
            return CommentErrors.CannotEditOthersComment;

        if (string.IsNullOrWhiteSpace(content))
            return CommentErrors.ContentRequired;

        if (content.Length > TechFlowConstants.Validation.MaxDescriptionLength)
            return CommentErrors.ContentTooLong;

        Content = content.Trim();
        IsEdited = true;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Updated;
    }

    public Result<Deleted> Delete(Guid requestedByUserId)
    {
        if (!IsOwner(requestedByUserId))
            return CommentErrors.CannotDeleteOthersComment;

        return Result.Deleted;
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private bool IsOwner(Guid requestedByUserId) => UserId == requestedByUserId;
}