using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Models;
using TechFlow.Domain.Common;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(ApplicationDbContext context)
    : IRepository<T> where T : Entity
{
    protected readonly ApplicationDbContext Context = context;
    protected readonly DbSet<T> DbSet = context.Set<T>();

    // ── Single Entity 

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await DbSet.FindAsync([id], ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await DbSet.AnyAsync(e => e.Id == id, ct);

    // ── Collections 

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await DbSet.AsNoTracking().ToListAsync(ct);

    public async Task<PaginatedList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default)
    {
        var count = await DbSet.CountAsync(ct);

        var items = await DbSet
            .AsNoTracking()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedList<T>(items, count, pageNumber, pageSize);
    }

    // ── Write 

    public void Add(T entity)    => DbSet.Add(entity);
    public void Update(T entity) => DbSet.Update(entity);
    public void Remove(T entity) => DbSet.Remove(entity);
}
