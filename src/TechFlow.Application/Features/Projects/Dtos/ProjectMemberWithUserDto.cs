namespace TechFlow.Application.Features.Projects.Dtos;

public sealed record ProjectMemberWithUserDto(
    Guid Id,
    Guid ProjectId,
    Guid UserId,
    string FirstName,
    string LastName,
    string Email,
    string? AvatarUrl,
    Guid AddedByUserId,
    DateTimeOffset AddedAt)
{
    public string FullName => $"{FirstName} {LastName}";
}