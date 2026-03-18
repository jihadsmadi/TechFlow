using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Sprints;
using TechFlow.Domain.Sprints.ValueObjects;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class SprintRepository(ApplicationDbContext context)
    : Repository<Sprint>(context), ISprintRepository
{
    public async Task<Sprint?> GetByIdWithItemsAsync(Guid sprintId, CancellationToken ct = default)
        => await context.Sprints
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == sprintId, ct);

    public async Task<Sprint?> GetActiveByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await context.Sprints
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s =>
                s.ProjectId == projectId &&
                s.Status == SprintStatus.Active, ct);

    public async Task<List<Sprint>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await context.Sprints
            .AsNoTracking()
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.SprintNumber)
            .ToListAsync(ct);

    public async Task<int> GetNextSprintNumberAsync(Guid projectId, CancellationToken ct = default)
    {
        var max = await context.Sprints
            .Where(s => s.ProjectId == projectId)
            .MaxAsync(s => (int?)s.SprintNumber, ct);

        return (max ?? 0) + 1;
    }

    public async Task<bool> HasActiveSprintAsync(Guid projectId, CancellationToken ct = default)
        => await context.Sprints
            .AnyAsync(s =>
                s.ProjectId == projectId &&
                s.Status == SprintStatus.Active, ct);

    public async Task<Sprint?> GetNextPlannedSprintAsync(Guid projectId, CancellationToken ct = default)
        => await context.Sprints
            .Include(s => s.Items)
            .Where(s =>
                s.ProjectId == projectId &&
                s.Status == SprintStatus.Planned)
            .OrderBy(s => s.SprintNumber)
            .FirstOrDefaultAsync(ct);

    public void MarkSprintItemAsAdded(SprintItem item)
        => context.Entry(item).State = EntityState.Added;
}