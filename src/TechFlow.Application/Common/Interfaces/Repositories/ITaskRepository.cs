using TechFlow.Domain.Tasks.Subtasks;

using Task = TechFlow.Domain.Tasks.Task;
namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface ITaskRepository : IRepository<Task>
{
    Task<Task?> GetByIdWithSubtasksAsync(Guid taskId, CancellationToken ct = default);
    Task<Task?> GetByIdWithAssignmentsAsync(Guid taskId, CancellationToken ct = default);
    Task<List<Task>> GetByListIdAsync(Guid listId, CancellationToken ct = default);
    Task<List<Task>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<List<Task>> GetByAssigneeAsync(Guid userId, Guid companyId, bool includeCompleted, CancellationToken ct = default);
    Task<double> GetMaxDisplayOrderInListAsync(Guid listId, CancellationToken ct = default);
    Task<List<Task>> GetByIdsAsync(IEnumerable<Guid> taskIds, CancellationToken ct = default);
    Task<List<Task>> GetBacklogTasksAsync(Guid projectId, CancellationToken ct = default);
    Task<Dictionary<Guid, (int Total, int Completed)>> GetCountsBySprintIdsAsync(IEnumerable<Guid> sprintIds, CancellationToken ct = default);
    Task<Task?> GetByIdWithDetailsAsync(Guid taskId, CancellationToken ct);
    void MarkSubtaskAsAdded(Subtask subtask);
}
