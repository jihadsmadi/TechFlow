using TechFlow.Domain.Invitations;

namespace TechFlow.Application.Features.Invitations.Dtos;

public sealed class InvitationDto
{
    public Guid Id { get; init; }
    public Guid CompanyId { get; init; }
    public Guid InvitedByUserId { get; init; }
    public Guid RoleId { get; init; }
    public string Email { get; init; } = string.Empty;
    public Guid? ProjectId { get; init; }
    public InvitationType Type { get; init; }
    public DateTimeOffset ExpiresAt { get; init; }
    public bool IsUsed { get; init; }
    public DateTimeOffset? UsedAt { get; init; }
    public bool IsRevoked { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}