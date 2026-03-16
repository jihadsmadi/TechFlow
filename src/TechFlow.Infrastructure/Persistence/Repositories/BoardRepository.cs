using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Boards;
using TechFlow.Infrastructure.Persistence;

namespace TechFlow.Infrastructure.Persistence.Repositories;


public sealed class BoardRepository(ApplicationDbContext context) : Repository<Board>(context), IBoardRepository
{
    public async Task<Board?> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
    => await context.Boards
        .FirstOrDefaultAsync(b => b.ProjectId == projectId, ct);
    public async Task<List?> GetListByIdAsync(Guid listId, CancellationToken ct = default)
     => await context.Lists.FirstOrDefaultAsync(l => l.Id == listId, ct);
    public async Task<Board?> GetByProjectIdWithListsAsync(
     Guid projectId, CancellationToken ct = default)
        => await context.Boards
            .Include(b => b.Lists.OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(b => b.ProjectId == projectId, ct);

    public async Task<Board?> GetByIdWithListsAsync(Guid boardId, CancellationToken ct = default)
        => await context.Boards
            .Include(b => b.Lists.OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(b => b.Id == boardId, ct);

    public async Task<bool> ExistsByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await context.Boards.AnyAsync(b => b.ProjectId == projectId, ct);
    public async Task<bool> ListBelongsToProjectAsync(Guid listId, Guid projectId, CancellationToken ct = default)
    => await context.Boards
        .AnyAsync(b => b.ProjectId == projectId && b.Lists.Any(l => l.Id == listId), ct);
    public void MarkListAsAdded(List list)
    => context.Entry(list).State = EntityState.Added;
}
