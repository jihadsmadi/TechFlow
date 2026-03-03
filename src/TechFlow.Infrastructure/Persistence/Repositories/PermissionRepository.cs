using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Permissions;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class PermissionRepository(ApplicationDbContext context)
    : Repository<Permission>(context), IPermissionRepository
{
    public async Task<Permission?> GetByNameAsync(string name, CancellationToken ct = default)
        => await DbSet
            .FirstOrDefaultAsync(p => p.Name == name.ToLower(), ct);

    public async Task<IReadOnlyList<Permission>> GetByGroupAsync(string group, CancellationToken ct = default)
        => await DbSet
            .AsNoTracking()
            .Where(p => p.Group == group)
            .OrderBy(p => p.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await DbSet
            .AnyAsync(p => p.Name == name.ToLower(), ct);

    public async Task<IReadOnlyList<Permission>> GetByNamesAsync(
        IEnumerable<string> names,
        CancellationToken ct = default)
    {
        var normalized = names.Select(n => n.ToLower()).ToList();

        return await DbSet
            .AsNoTracking()
            .Where(p => normalized.Contains(p.Name))
            .ToListAsync(ct);
    }
}
