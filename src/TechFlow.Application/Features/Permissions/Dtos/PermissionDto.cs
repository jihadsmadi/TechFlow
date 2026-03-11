namespace TechFlow.Application.Features.Permissions.Dtos;

public sealed class PermissionDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Group { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}