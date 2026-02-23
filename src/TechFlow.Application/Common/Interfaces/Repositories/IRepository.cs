using TechFlow.Domain.Common;
using TechFlow.Application.Common.Models;

namespace TechFlow.Application.Common.Interfaces.Repositories;

/// <summary>
/// Generic repository base — provides common operations for every entity.
/// Specific repositories extend this with their own queries.
/// </summary>
public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    Task<PaginatedList<T>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken ct = default);

    void Add(T entity);

    void Update(T entity);

    void Remove(T entity);
}