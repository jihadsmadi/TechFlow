using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Companies.ValueObjects;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public sealed class CompanyRepository(ApplicationDbContext context)
    : Repository<Company>(context), ICompanyRepository
{
    public async Task<Company?> GetBySlugAsync(string slug, CancellationToken ct = default)
        => await DbSet
            .Include(c => c.FeatureFlags)
            .FirstOrDefaultAsync(c => c.Slug.Value == slug, ct);

    public async Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct = default)
        => await DbSet
            .AnyAsync(c => c.Slug.Value == slug, ct);
}
