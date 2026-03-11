namespace TechFlow.Application.Features.Users.Dtos;

public sealed record UserProfileDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string? AvatarUrl,
    bool IsActive,
    Guid CompanyId,
    string Role,
    UserPreferencesDto Preferences);
