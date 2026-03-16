using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;


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
    public async Task<double> GetMaxDisplayOrderInListAsync(Guid listId, CancellationToken ct = default)
    {
        var max = await context.Tasks
            .Where(t => t.ListId == listId)
            .MaxAsync(t => (double?)t.DisplayOrder, ct);

        return max ?? 0;
    }

    public void MarkSubtaskAsAdded(Domain.Tasks.Subtasks.Subtask subtask)
        => context.Entry(subtask).State = Microsoft.EntityFrameworkCore.EntityState.Added;
}