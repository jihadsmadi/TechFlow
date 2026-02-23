using TechFlow.Domain.Common.Results;

namespace TechFlow.Domain.Tasks.Comments;

public static class CommentErrors
{
    public static readonly Error TaskIdRequired =
        Error.Validation("Comment.TaskIdRequired", "Task ID is required.");

    public static readonly Error UserIdRequired =
        Error.Validation("Comment.UserIdRequired", "User ID is required.");

    public static readonly Error ContentRequired =
        Error.Validation("Comment.ContentRequired", "Comment content is required.");

    public static readonly Error ContentTooLong =
        Error.Validation("Comment.ContentTooLong", "Comment is too long.");

    public static readonly Error NotFound =
        Error.NotFound("Comment.NotFound", "Comment was not found.");

    public static readonly Error CannotEditOthersComment =
        Error.Forbidden("Comment.CannotEditOthersComment", "You can only edit your own comments.");

    public static readonly Error CannotDeleteOthersComment =
        Error.Forbidden("Comment.CannotDeleteOthersComment", "You can only delete your own comments.");
}
