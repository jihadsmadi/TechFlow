using TechFlow.Domain.Companies;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface ICompanyRepository : IRepository<Company>
{
    Task<Company?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default);
    Task<bool> ExistsByContactEmailAsync(string email, CancellationToken ct = default);
}