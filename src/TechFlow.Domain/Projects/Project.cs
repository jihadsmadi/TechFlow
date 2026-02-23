using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Constants;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects.Events;
using TechFlow.Domain.Projects.ProjectSettings;
using TechFlow.Domain.Projects.ValueObjects;

namespace TechFlow.Domain.Projects;

public sealed class Project : AuditableEntity
{
    public Guid CompanyId { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Status { get; private set; } = ProjectStatus.Active;
    public ProjectColor Color { get; private set; } = null!;
    public DateTimeOffset? StartDate { get; private set; }
    public DateTimeOffset? EndDate { get; private set; }
    public bool IsArchived { get; private set; } = false;
    public DateTimeOffset? ArchivedAt { get; private set; }

    // Owned entity — auto created on project creation
    public ProjectSetting Settings { get; private set; } = null!;

    // Members
    private readonly List<ProjectMember> _members = [];
    public IReadOnlyList<ProjectMember> Members => _members.AsReadOnly();

    private Project() { }

    private Project(
        Guid id,
        Guid companyId,
        Guid createdByUserId,
        string name,
        string? description,
        ProjectColor color,
        DateTimeOffset? startDate,
        DateTimeOffset? endDate)
        : base(id)
    {
        CompanyId = companyId;
        CreatedByUserId = createdByUserId;
        Name = name;
        Description = description;
        Color = color;
        StartDate = startDate;
        EndDate = endDate;
        Settings = ProjectSetting.CreateDefault();
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<Project> Create(
        Guid companyId,
        Guid createdByUserId,
        string name,
        string? description = null,
        string? color = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null)
    {
        if (!IsValidId(companyId))
            return ProjectErrors.CompanyIdRequired;

        if (!IsValidId(createdByUserId))
            return ProjectErrors.CreatedByRequired;

        if (string.IsNullOrWhiteSpace(name))
            return ProjectErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return ProjectErrors.NameTooLong;

        if (!IsValidDateRange(startDate, endDate))
            return ProjectErrors.InvalidDateRange;

        var colorResult = color is not null
            ? ProjectColor.Create(color)
            : ProjectColor.Default();  // use default if not provided

        if (colorResult.IsFailure)
            return colorResult.TopError;

        var project = new Project(
            id: Guid.NewGuid(),
            companyId: companyId,
            createdByUserId: createdByUserId,
            name: name.Trim(),
            description: description?.Trim(),
            color: colorResult.Value,
            startDate: startDate,
            endDate: endDate
        );

        project.AddDomainEvent(new ProjectCreatedEvent(project.Id, project.CompanyId, project.Name));

        return project;
    }

    // ── Business ───────────────────────────────────────────────────────────────

    public Result<Updated> Update(
        string name,
        string? description = null,
        string? color = null,
        DateTimeOffset? startDate = null,
        DateTimeOffset? endDate = null)
    {
        if (IsArchived)
            return ProjectErrors.CannotModifyArchived;

        if (string.IsNullOrWhiteSpace(name))
            return ProjectErrors.NameRequired;

        if (name.Length > TechFlowConstants.Validation.MaxNameLength)
            return ProjectErrors.NameTooLong;

        if (!IsValidDateRange(startDate, endDate))
            return ProjectErrors.InvalidDateRange;

        if (color is not null)
        {
            var colorResult = ProjectColor.Create(color);
            if (colorResult.IsFailure)
                return colorResult.TopError;

            Color = colorResult.Value;
        }

        Name = name.Trim();
        Description = description?.Trim();
        StartDate = startDate;
        EndDate = endDate;

        return Result.Updated;
    }

    public Result<Updated> ChangeStatus(string status)
    {
        if (IsArchived)
            return ProjectErrors.CannotModifyArchived;

        if (!ProjectStatus.IsValid(status))
            return ProjectErrors.InvalidStatus(status);

        Status = status;
        return Result.Updated;
    }

    public Result<Updated> Archive()
    {
        if (IsArchived)
            return ProjectErrors.AlreadyArchived;

        IsArchived = true;
        ArchivedAt = DateTimeOffset.UtcNow;
        Status = ProjectStatus.Archived;

        AddDomainEvent(new ProjectArchivedEvent(Id, CompanyId));

        return Result.Updated;
    }

    public Result<Updated> Restore()
    {
        if (!IsArchived)
            return ProjectErrors.NotArchived;

        IsArchived = false;
        ArchivedAt = null;
        Status = ProjectStatus.Active;

        return Result.Updated;
    }

    public Result<Updated> AddMember(Guid userId, Guid addedByUserId)
    {
        if (IsArchived)
            return ProjectErrors.CannotModifyArchived;

        if (IsMember(userId))
            return ProjectErrors.MemberAlreadyExists;

        var memberResult = ProjectMember.Create(Id, userId, addedByUserId);
        if (memberResult.IsFailure)
            return memberResult.TopError;

        _members.Add(memberResult.Value);

        AddDomainEvent(new ProjectMemberAddedEvent(Id, userId));

        return Result.Updated;
    }

    public Result<Updated> RemoveMember(Guid userId)
    {
        if (IsArchived)
            return ProjectErrors.CannotModifyArchived;

        if (userId == CreatedByUserId)
            return ProjectErrors.CannotRemoveOwner;

        var member = _members.FirstOrDefault(m => m.UserId == userId);

        if (member is null)
            return ProjectErrors.MemberNotFound;

        _members.Remove(member);

        AddDomainEvent(new ProjectMemberRemovedEvent(Id, userId));

        return Result.Updated;
    }

    public bool IsMember(Guid userId) =>
        _members.Any(m => m.UserId == userId) || userId == CreatedByUserId;

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;

    private static bool IsValidDateRange(DateTimeOffset? start, DateTimeOffset? end) =>
        start is null || end is null || start <= end;
}