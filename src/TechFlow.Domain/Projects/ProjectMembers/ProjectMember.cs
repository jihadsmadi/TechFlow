using TechFlow.Domain.Common;
using TechFlow.Domain.Common.Results;
using TechFlow.Domain.Projects.ProjectMembers;

namespace TechFlow.Domain.Projects;

/// <summary>
/// Represents a user's membership in a project.
/// Separate from UserRole — membership answers "who is on this project?"
/// UserRole answers "what can they do?"
/// A user can be a member without a specific role.
/// </summary>
public sealed class ProjectMember : Entity
{
    public Guid ProjectId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AddedByUserId { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }

    private ProjectMember() { }

    private ProjectMember(Guid id, Guid projectId, Guid userId, Guid addedByUserId)
        : base(id)
    {
        ProjectId = projectId;
        UserId = userId;
        AddedByUserId = addedByUserId;
        AddedAt = DateTimeOffset.UtcNow;
    }

    // ── Factory ────────────────────────────────────────────────────────────────

    public static Result<ProjectMember> Create(Guid projectId, Guid userId, Guid addedByUserId)
    {
        if (!IsValidId(projectId))
            return ProjectMemberErrors.ProjectIdRequired;

        if (!IsValidId(userId))
            return ProjectMemberErrors.UserIdRequired;

        if (!IsValidId(addedByUserId))
            return ProjectMemberErrors.AddedByRequired;

        return new ProjectMember(Guid.NewGuid(), projectId, userId, addedByUserId);
    }

    // ── Private Validation ─────────────────────────────────────────────────────

    private static bool IsValidId(Guid id) => id != Guid.Empty;
}