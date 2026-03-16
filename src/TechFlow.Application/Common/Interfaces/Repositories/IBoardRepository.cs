using TechFlow.Domain.Boards;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface IBoardRepository: IRepository<Board>
{
    Task<Board?> GetByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<List?> GetListByIdAsync(Guid listId, CancellationToken ct = default);

    /// <summary>
    /// Gets board with lists loaded — needed for all board operations.
    /// </summary>
    Task<Board?> GetByProjectIdWithListsAsync(Guid projectId, CancellationToken ct = default);
    Task<Board?> GetByIdWithListsAsync(Guid boardId, CancellationToken ct = default);

    Task<bool> ExistsByProjectIdAsync(Guid projectId, CancellationToken ct = default);
    Task<bool> ListBelongsToProjectAsync(Guid listId, Guid projectId, CancellationToken ct = default);
    void MarkListAsAdded(List list);


}
