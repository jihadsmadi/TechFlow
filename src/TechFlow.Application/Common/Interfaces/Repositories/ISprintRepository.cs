using TechFlow.Domain.Sprints;
using TechFlow.Domain.Sprints.ValueObjects;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface ISprintRepository : IRepository<Sprint>
{
    Task<Sprint?> GetByIdWithItemsAsync(Guid sprintId, CancellationToken ct = default);
    Task<Sprint?> GetActiveByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<List<Sprint>> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<int> GetNextSprintNumberAsync(Guid projectId, CancellationToken ct = default);
    Task<bool> HasActiveSprintAsync(Guid projectId, CancellationToken ct = default);
    Task<Sprint?> GetNextPlannedSprintAsync(Guid projectId, CancellationToken ct = default);
    void MarkSprintItemAsAdded(SprintItem item);
}
