namespace TechFlow.Application.Features.Roles.Dtos;

public sealed class RoleSummaryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsSystemRole { get; init; }
    public int PermissionCount { get; init; }
}