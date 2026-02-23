using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Tasks.Attachments;
using TechFlow.Domain.Tasks.Comments;
using TechFlow.Domain.Tasks.Comments.Events;
using TechFlow.Domain.Tasks.CustomeFields;
using TechFlow.Domain.Tasks.Events;
using TechFlow.Domain.Tasks.TaskAssignments;
using TechFlow.Domain.Tasks.ValueObjects;

namespace TechFlow.Domain.Tasks;

public sealed class Task : AuditableEntity
{
    // ── Identity & Location ────────────────────────────────────────────────────
    public Guid ListId { get; private set; }
    public Guid CompanyId { get; private set; }  // denormalized
    public Guid ProjectId { get; private set; }  // denormalized
    public Guid CreatedByUserId { get; private set; }
    public Guid? ParentTaskId { get; private set; }  // null = top-level task

    // ── Core Fields ────────────────────────────────────────────────────────────
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Priority { get; private set; } = ValueObjects.Priority.Medium;
    public string Type { get; private set; } = TaskType.Feature;
    public int DisplayOrder { get; private set; }

    // ── Time ───────────────────────────────────────────────────────────────────
    public DateTimeOffset? DueDate { get; private set; }
    public int? EstimatedMinutes { get; private set; }
    public int? ActualMinutes { get; private set; }

    // ── Completion ─────────────────────────────────────────────────────────────
    public bool IsCompleted { get; private set; } = false;
    public DateTimeOffset? CompletedAt { get; private set; }

    // ── Collections ────────────────────────────────────────────────────────────
    private readonly List<TaskAssignment> _assignments = [];
    private readonly List<Comment> _comments = [];
    private readonly List<Attachment> _attachments = [];
    private readonly List<CustomFieldValue> _customFields = [];
    private readonly List<Task> _subtasks = [];

    public IReadOnlyList<TaskAssignment> Assignments => _assignments.AsReadOnly();
    public IReadOnlyList<Comment> Comments => _comments.AsReadOnly();
    public IReadOnlyList<Attachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyList<CustomFieldValue> CustomFields => _customFields.AsReadOnly();
    public IReadOnlyList<Task> Subtasks => _subtasks.AsReadOnly();

    public bool IsSubtask => ParentTaskId.HasValue;

    private Task() { }

    private Task(
        Guid id,
        Guid listId,
        Guid companyId,
        Guid projectId,
        Guid createdByUserId,
        Guid? parentTaskId,
        string title,
        string priority,
        string type,
        int displayOrder)
        : base(id)
    {
        ListId = listId;
        CompanyId = companyId;
        ProjectId = projectId;
        CreatedByUserId = createdByUserId;
        ParentTaskId = parentTaskId;
        Title = title;
        Priority = priority;
        Type = type;
        DisplayOrder = displayOrder;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<Task> Create(
        Guid listId,
        Guid companyId,
        Guid projectId,
        Guid createdByUserId,
        string title,
        string priority = ValueObjects.Priority.Medium,
        string type = TaskType.Feature,
        int displayOrder = 0,
        Guid? parentTaskId = null,
        bool allowSubtasks = true)
    {
        if (!IsValidId(listId))
            return TaskErrors.ListIdRequired;

        if (!IsValidId(companyId))
            return TaskErrors.CompanyIdRequired;

        if (!IsValidId(projectId))
            return TaskErrors.ProjectIdRequired;

        if (!IsValidId(createdByUserId))
            return TaskErrors.CreatedByRequired;

        if (string.IsNullOrWhiteSpace(title))
            return TaskErrors.TitleRequired;

        if (title.Length > TechFlowConstants.Validation.MaxNameLength)
            return TaskErrors.TitleTooLong;

        if (!ValueObjects.Priority.IsValid(priority))
            return TaskErrors.InvalidPriority(priority);

        if (!TaskType.IsValid(type))
            return TaskErrors.InvalidType(type);

        // Subtask rules
        if (parentTaskId.HasValue && !allowSubtasks)
            return TaskErrors.SubtasksNotAllowed;

        var task = new Task(
            id: Guid.NewGuid(),
            listId: listId,
            companyId: companyId,
            projectId: projectId,
            createdByUserId: createdByUserId,
            parentTaskId: parentTaskId,
            title: title.Trim(),
            priority: priority,
            type: type,
            displayOrder: displayOrder
        );

        task.AddDomainEvent(new TaskCreatedEvent(task.Id, task.ProjectId, task.CompanyId, task.Title));

        return task;
    }

    // ── Business — Core ────────────────────────────────────────────────────────

    public Result<Updated> Update(
        string title,
        string? description = null,
        string? priority = null,
        string? type = null,
        DateTimeOffset? dueDate = null,
        int? estimatedMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            return TaskErrors.TitleRequired;

        if (title.Length > TechFlowConstants.Validation.MaxNameLength)
            return TaskErrors.TitleTooLong;

        if (priority is not null && !ValueObjects.Priority.IsValid(priority))
            return TaskErrors.InvalidPriority(priority);

        if (type is not null && !TaskType.IsValid(type))
            return TaskErrors.InvalidType(type);

        if (estimatedMinutes.HasValue && !IsValidMinutes(estimatedMinutes.Value))
            return TaskErrors.InvalidEstimate;

        Title = title.Trim();
        Description = description?.Trim();
        Priority = priority ?? Priority;
        Type = type ?? Type;
        DueDate = dueDate;
        EstimatedMinutes = estimatedMinutes;

        return Result.Updated;
    }

    public Result<Updated> MoveToList(Guid newListId, int newDisplayOrder)
    {
        if (!IsValidId(newListId))
            return TaskErrors.ListIdRequired;

        var previousListId = ListId;

        ListId = newListId;
        DisplayOrder = newDisplayOrder;

        AddDomainEvent(new TaskMovedEvent(Id, previousListId, newListId, ProjectId));

        return Result.Updated;
    }

    public Result<Updated> Complete()
    {
        if (IsCompleted)
            return TaskErrors.AlreadyCompleted;

        IsCompleted = true;
        CompletedAt = DateTimeOffset.UtcNow;

        AddDomainEvent(new TaskCompletedEvent(Id, ProjectId, CompanyId));

        return Result.Updated;
    }

    public Result<Updated> Reopen()
    {
        if (!IsCompleted)
            return TaskErrors.NotCompleted;

        IsCompleted = false;
        CompletedAt = null;

        return Result.Updated;
    }

    public Result<Updated> LogActualTime(int minutes)
    {
        if (!IsValidMinutes(minutes))
            return TaskErrors.InvalidActualTime;

        ActualMinutes = minutes;
        return Result.Updated;
    }

    // ── Business — Assignments ─────────────────────────────────────────────────

    public Result<Updated> AssignUser(Guid userId, Guid assignedByUserId)
    {
        if (IsAssigned(userId))
            return TaskErrors.AlreadyAssigned;

        var result = TaskAssignment.Create(Id, userId, assignedByUserId);
        if (result.IsFailure)
            return result.TopError;

        _assignments.Add(result.Value);

        AddDomainEvent(new TaskAssignedEvent(Id, userId, ProjectId));

        return Result.Updated;
    }

    public Result<Updated> UnassignUser(Guid userId)
    {
        var assignment = _assignments.FirstOrDefault(a => a.UserId == userId);

        if (assignment is null)
            return TaskErrors.AssignmentNotFound;

        _assignments.Remove(assignment);

        AddDomainEvent(new TaskUnassignedEvent(Id, userId));

        return Result.Updated;
    }

    // ── Business — Comments ────────────────────────────────────────────────────

    public Result<Comment> AddComment(Guid userId, string content)
    {
        var result = Comment.Create(Id, userId, content);
        if (result.IsFailure)
            return result.TopError;

        _comments.Add(result.Value);

        AddDomainEvent(new CommentAddedEvent(Id, userId, ProjectId));

        return result.Value;
    }

    public Result<Deleted> DeleteComment(Guid commentId, Guid requestedByUserId)
    {
        var comment = _comments.FirstOrDefault(c => c.Id == commentId);

        if (comment is null)
            return CommentErrors.NotFound;

        var result = comment.Delete(requestedByUserId);
        if (result.IsFailure)
            return result.TopError;

        _comments.Remove(comment);
        return Result.Deleted;
    }

    // ── Business — Attachments ─────────────────────────────────────────────────

    public Result<Attachment> AddAttachment(
        Guid uploadedByUserId,
        string fileName,
        string fileUrl,
        string fileType,
        long fileSizeBytes)
    {
        var result = Attachment.Create(Id, uploadedByUserId, fileName, fileUrl, fileType, fileSizeBytes);
        if (result.IsFailure)
            return result.TopError;

        _attachments.Add(result.Value);
        return result.Value;
    }

    public Result<Deleted> RemoveAttachment(Guid attachmentId)
    {
        var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);

        if (attachment is null)
            return AttachmentErrors.NotFound;

        _attachments.Remove(attachment);
        return Result.Deleted;
    }

    // ── Business — Custom Fields ───────────────────────────────────────────────

    public Result<Updated> SetCustomFieldValue(Guid definitionId, string value)
    {
        var existing = _customFields.FirstOrDefault(cf => cf.CustomFieldDefinitionId == definitionId);

        if (existing is not null)
            return existing.UpdateValue(value);

        var result = CustomFieldValue.Create(Id, definitionId, value);
        if (result.IsFailure)
            return result.TopError;

        _customFields.Add(result.Value);
        return Result.Updated;
    }

    // ── Business — Subtasks ────────────────────────────────────────────────────

    public Result<Task> AddSubtask(
        Guid createdByUserId,
        string title,
        bool allowSubtasks = true)
    {
        if (IsSubtask)
            return TaskErrors.CannotNestSubtask;

        var result = Task.Create(
            listId: ListId,
            companyId: CompanyId,
            projectId: ProjectId,
            createdByUserId: createdByUserId,
            title: title,
            parentTaskId: Id,
            allowSubtasks: allowSubtasks
        );

        if (result.IsFailure)
            return result.TopError;

        _subtasks.Add(result.Value);
        return result.Value;
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    public bool IsAssigned(Guid userId) =>
        _assignments.Any(a => a.UserId == userId);

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
    private static bool IsValidMinutes(int mins) => mins > 0;
}