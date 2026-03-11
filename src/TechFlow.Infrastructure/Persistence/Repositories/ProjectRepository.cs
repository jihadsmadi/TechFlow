using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Models;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Projects;
using TechFlow.Infrastructure.Persistence;

namespace TechFlow.Infrastructure.Persistence.Repositories;

public class ProjectRepository(ApplicationDbContext context)
    : Repository<Project>(context), IProjectRepository
{
    public async Task<List<Project>> GetByCompanyAsync(Guid companyId, Guid? userId, bool includeArchived, CancellationToken ct = default)
    {
        return await Context.Projects.Where(p => p.CompanyId == companyId)
            .Where(p => includeArchived || !p.IsArchived)
            .Where(p => userId == null || p.Members.Any(m => m.UserId == userId))
            .ToListAsync(ct);
    }

    public async Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
    {
        return await Context.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<List<Project>> GetByMemberAsync(Guid userId, CancellationToken ct = default)
    {
        return await Context.Projects.Where(p => p.Members.Any(m => m.UserId == userId)).ToListAsync(ct);
    }

    public async Task<bool> NameExistsInCompanyAsync(Guid companyId, string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        return await Context.Projects.AnyAsync(p => p.CompanyId == companyId
        && p.Name == name
        && (excludeId == null || p.Id != excludeId), ct);
    }
    public async Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default)
    => await context.ProjectMembers
        .AnyAsync(m => m.ProjectId == projectId && m.UserId == userId, ct)
        || await context.Projects
        .AnyAsync(p => p.Id == projectId && p.CreatedByUserId == userId, ct);
}
