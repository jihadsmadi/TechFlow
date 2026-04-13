using TechFlow.Application.Features.Invitations.Dtos;
using TechFlow.Domain.Invitations;

namespace TechFlow.Application.Features.Invitations.Mappers;

public static class InvitationMapper
{
    public static InvitationDto ToDto(this Invitation entity) => new()
    {
        Id = entity.Id,
        CompanyId = entity.CompanyId,
        InvitedByUserId = entity.InvitedByUserId,
        RoleId = entity.RoleId,
        Email = entity.Email,
        ProjectId = entity.ProjectId,
        Type = entity.Type,
        ExpiresAt = entity.ExpiresAt,
        IsUsed = entity.IsUsed,
        UsedAt = entity.UsedAt,
        IsRevoked = entity.IsRevoked,
        CreatedAt = entity.CreatedAtUtc
    };

    public static List<InvitationDto> ToDtos(this IEnumerable<Invitation> entities) =>
        [.. entities.Select(e => e.ToDto())];
}