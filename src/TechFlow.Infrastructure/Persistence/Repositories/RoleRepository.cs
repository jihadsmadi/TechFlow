using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Roles;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class RoleRepository(ApplicationDbContext context)
    : Repository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameWithPermessionsAsync(string name, CancellationToken ct = default)
        => await DbSet
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == name, ct);

    public async Task<Role?> GetWithPermissionsAsync(Guid roleId, CancellationToken ct = default)
        => await DbSet
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == roleId, ct);

    public async Task<IReadOnlyList<Role>> GetAllWithPermissionsAsync(CancellationToken ct = default)
        => await DbSet
            .AsNoTracking()
            .Include(r => r.Permissions)
            .OrderBy(r => r.Name)
            .ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => await DbSet
            .AnyAsync(r => r.Name == name, ct);
}
