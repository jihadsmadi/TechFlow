using TechFlow.Domain.Permissions;

namespace TechFlow.Application.Common.Interfaces.Repositories;

/// <summary>
/// Permission-specific queries beyond the generic IRepository base.
/// </summary>
public interface IPermissionRepository : IRepository<Permission>
{
    Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default);

    Task<IReadOnlyList<Permission>> GetByGroupAsync(string group, CancellationToken ct = default);

    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);

    Task<IReadOnlyList<Permission>> GetByNamesAsync(IEnumerable<string> names, CancellationToken ct = default);
}