using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Sprints.ValueObjects;


namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class TaskRepository(ApplicationDbContext context) : Repository<Domain.Tasks.Task>(context), ITaskRepository
{
    public async Task<Domain.Tasks.Task?> GetByIdWithSubtasksAsync(Guid taskId, CancellationToken ct = default)
        => await context.Tasks
            .Include(t => t.Subtasks)
            .FirstOrDefaultAsync(t => t.Id == taskId, ct);
    public async Task<Domain.Tasks.Task?> GetByIdWithAssignmentsAsync(Guid taskId, CancellationToken ct = default)
        => await context.Tasks
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t => t.Id == taskId, ct);
    public async Task<List<Domain.Tasks.Task>> GetByListIdAsync(Guid listId, CancellationToken ct = default)
      => await context.Tasks
          .AsNoTracking()
          .Include(t => t.Subtasks)
          .Where(t => t.ListId == listId)
          .OrderBy(t => t.DisplayOrder)
          .ToListAsync(ct);

    public async Task<List<Domain.Tasks.Task>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default)
        => await context.Tasks
            .AsNoTracking()
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.DisplayOrder)
            .ToListAsync(ct);
    public async Task<List<Domain.Tasks.Task>> GetByIdsAsync(
        IEnumerable<Guid> taskIds,
        CancellationToken ct = default)
        => await context.Tasks
            .AsNoTracking()
            .Include(t => t.Subtasks)
            .Where(t => taskIds.Contains(t.Id))
            .ToListAsync(ct);
    public async Task<List<Domain.Tasks.Task>> GetByAssigneeAsync(
        Guid userId,
        Guid companyId,
        bool includeCompleted,
        CancellationToken ct = default)
        => await context.Tasks
            .AsNoTracking()
            .Include(t => t.Subtasks)
            .Where(t =>
                t.CompanyId == companyId &&
                (includeCompleted || !t.IsCompleted) &&
                t.Assignments.Any(a => a.UserId == userId))
            .OrderBy(t => t.DueDate)
            .ThenBy(t => t.Priority)
            .ToListAsync(ct);
    public async Task<List<Domain.Tasks.Task>> GetBacklogTasksAsync(
    Guid projectId,
    CancellationToken ct = default)
    => await context.Tasks
        .AsNoTracking()
        .Include(t => t.Subtasks)
        .Where(t =>
            t.ProjectId == projectId &&
            !t.IsCompleted &&
            !context.SprintItems.Any(si =>
                si.TaskId == t.Id &&
                context.Sprints.Any(s =>
                    s.Id == si.SprintId &&
                    (s.Status == SprintStatus.Active ||
                     s.Status == SprintStatus.Planned))))
        .OrderBy(t => t.DisplayOrder)
        .ToListAsync(ct);

    public async Task<Dictionary<Guid, (int Total, int Completed)>> GetCountsBySprintIdsAsync(
        IEnumerable<Guid> sprintIds,
        CancellationToken ct = default)
    {
        var counts = await context.SprintItems
            .Where(si => sprintIds.Contains(si.SprintId))
            .Join(context.Tasks,
                si => si.TaskId,
                t => t.Id,
                (si, t) => new { si.SprintId, t.IsCompleted })
            .GroupBy(x => x.SprintId)
            .Select(g => new
            {
                SprintId = g.Key,
                Total = g.Count(),
                Completed = g.Count(x => x.IsCompleted)
            })
            .ToListAsync(ct);

        return counts.ToDictionary(
            x => x.SprintId,
            x => (x.Total, x.Completed));
    }

    public async Task<double> GetMaxDisplayOrderInListAsync(Guid listId, CancellationToken ct = default)
    {
        var max = await context.Tasks
            .Where(t => t.ListId == listId)
            .MaxAsync(t => (double?)t.DisplayOrder, ct);

        return max ?? 0;
    }
    public async Task<Domain.Tasks.Task?> GetByIdWithDetailsAsync(Guid taskId, CancellationToken ct)
    {
        return await context.Tasks
            .Include(t => t.Subtasks)
            .Include(t => t.Assignments)
            .FirstOrDefaultAsync(t => t.Id == taskId, ct);
    }
    public void MarkSubtaskAsAdded(Domain.Tasks.Subtasks.Subtask subtask)
        => context.Entry(subtask).State = Microsoft.EntityFrameworkCore.EntityState.Added;
}