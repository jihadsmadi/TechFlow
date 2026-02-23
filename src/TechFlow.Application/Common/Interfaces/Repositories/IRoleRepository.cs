using TechFlow.Domain.Roles;

namespace TechFlow.Application.Common.Interfaces.Repositories;

/// <summary>
/// Role-specific queries beyond the generic IRepository base.
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<Role?> GetWithPermissionsAsync(Guid roleId, CancellationToken ct = default);
    Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}