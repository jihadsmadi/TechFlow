using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects.ProjectSettings;
using TechFlow.Domain.Sprints.Events;
using TechFlow.Domain.Sprints.ValueObjects;

namespace TechFlow.Domain.Sprints;

public sealed class Sprint : AuditableEntity
{
    // ── Identity
    public Guid ProjectId { get; private set; }
    public Guid CompanyId { get; private set; }
    public Guid CreatedByUserId { get; private set; }

    // ── Core Fields
    public int SprintNumber { get; private set; }
    public string? Name { get; private set; }
    public string? Goal { get; private set; }
    public string Status { get; private set; } = SprintStatus.Planned;

    // ── Dates
    public DateTimeOffset StartDate { get; private set; }
    public DateTimeOffset EndDate { get; private set; }
    public DateTimeOffset? ActualEndDate { get; private set; }

    // ── Lock
    public bool IsLocked { get; private set; } = false;

    // ── Items
    private readonly List<SprintItem> _items = [];
    public IReadOnlyList<SprintItem> Items => _items.AsReadOnly();

    private Sprint() { }

    private Sprint(
        Guid id,
        Guid projectId,
        Guid companyId,
        Guid createdByUserId,
        int sprintNumber,
        string? name,
        string? goal,
        DateTimeOffset startDate,
        DateTimeOffset endDate)
        : base(id)
    {
        ProjectId = projectId;
        CompanyId = companyId;
        CreatedByUserId = createdByUserId;
        SprintNumber = sprintNumber;
        Name = name;
        Goal = goal;
        StartDate = startDate;
        EndDate = endDate;
    }

    // ── Factory

    public static Result<Sprint> Create(
        Guid projectId,
        Guid companyId,
        Guid createdByUserId,
        int sprintNumber,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        string? name = null,
        string? goal = null)
    {
        if (projectId == Guid.Empty)
            return SprintErrors.ProjectIdRequired;

        if (endDate <= startDate)
            return SprintErrors.InvalidDates;

        if (name is not null && name.Length > TechFlowConstants.Validation.MaxNameLength)
            return SprintErrors.NameTooLong;

        var sprint = new Sprint(
            id: Guid.NewGuid(),
            projectId: projectId,
            companyId: companyId,
            createdByUserId: createdByUserId,
            sprintNumber: sprintNumber,
            name: name?.Trim(),
            goal: goal?.Trim(),
            startDate: startDate,
            endDate: endDate);

        sprint.AddDomainEvent(new SprintCreatedEvent(sprint.Id, sprint.ProjectId, sprint.SprintNumber));

        return sprint;
    }

    // ── Lifecycle

    public Result<Updated> Start(bool lockOnStart)
    {
        if (Status != SprintStatus.Planned)
            return SprintErrors.NotPlanned;

        Status = SprintStatus.Active;
        IsLocked = lockOnStart;

        AddDomainEvent(new SprintStartedEvent(Id, ProjectId));

        return Result.Updated;
    }

    public Result<Updated> End(string incompleteTasksAction)
    {
        if (Status != SprintStatus.Active)
            return SprintErrors.NotActive;

        if (!IncompleteTasksActionType.IsValid(incompleteTasksAction))
            return SprintErrors.InvalidIncompleteTasksAction;

        Status = SprintStatus.Completed;
        ActualEndDate = DateTimeOffset.UtcNow;

        AddDomainEvent(new SprintEndedEvent(Id, ProjectId, incompleteTasksAction));

        return Result.Updated;
    }

    public Result<Updated> Cancel()
    {
        if (Status == SprintStatus.Completed)
            return SprintErrors.AlreadyCompleted;

        if (Status == SprintStatus.Cancelled)
            return SprintErrors.AlreadyCancelled;

        Status = SprintStatus.Cancelled;

        AddDomainEvent(new SprintCancelledEvent(Id, ProjectId));

        return Result.Updated;
    }

    public Result<Updated> Update(
        string? name,
        string? goal,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate)
    {
        var newStart = startDate ?? StartDate;
        var newEnd = endDate ?? EndDate;

        if (newEnd <= newStart)
            return SprintErrors.InvalidDates;

        if (name is not null && name.Length > TechFlowConstants.Validation.MaxNameLength)
            return SprintErrors.NameTooLong;

        Name = name?.Trim() ?? Name;
        Goal = goal?.Trim() ?? Goal;
        StartDate = newStart;
        EndDate = newEnd;

        return Result.Updated;
    }

    // ── Tasks

    public Result<SprintItem> AddTask(Guid taskId)
    {
        if (IsLocked && Status == SprintStatus.Active)
            return SprintErrors.CannotModifyActiveSprint;

        if (Status == SprintStatus.Completed || Status == SprintStatus.Cancelled)
            return SprintErrors.AlreadyCompleted;

        if (HasTask(taskId))
            return SprintErrors.TaskAlreadyInSprint;

        var result = SprintItem.Create(Id, taskId);
        if (result.IsFailure)
            return result.TopError;

        _items.Add(result.Value);

        return result.Value;
    }

    public Result<Updated> RemoveTask(Guid taskId)
    {
        if (IsLocked && Status == SprintStatus.Active)
            return SprintErrors.CannotModifyActiveSprint;

        var item = FindItem(taskId);
        if (item is null)
            return SprintErrors.TaskNotInSprint;

        _items.Remove(item);

        return Result.Updated;
    }

    // ── Helpers

    public bool HasTask(Guid taskId) =>
        _items.Any(i => i.TaskId == taskId);

    public string DisplayName =>
        Name is not null ? $"Sprint {SprintNumber} — {Name}" : $"Sprint {SprintNumber}";

    private SprintItem? FindItem(Guid taskId) =>
        _items.FirstOrDefault(i => i.TaskId == taskId);
}
