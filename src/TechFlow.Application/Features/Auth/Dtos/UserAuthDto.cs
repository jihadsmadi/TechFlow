namespace TechFlow.Application.Features.Auth.Dtos;

public sealed record UserAuthDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    Guid CompanyId,
    string Role);