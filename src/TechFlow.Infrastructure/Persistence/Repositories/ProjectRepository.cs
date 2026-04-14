using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Models;
using TechFlow.Application.Features.Projects.Dtos;
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
    // ProjectRepository.cs
    public async Task<List<ProjectMemberWithUserDto>> GetMembersWithUserDetailsAsync(Guid projectId, CancellationToken ct)
    {
        var query = from member in Context.ProjectMembers
                    join user in Context.Users on member.UserId equals user.Id
                    where member.ProjectId == projectId
                    select new ProjectMemberWithUserDto(
                        member.Id,
                        member.ProjectId,
                        member.UserId,
                        user.FirstName,
                        user.LastName,
                        user.Email,
                        user.AvatarUrl,
                        member.AddedByUserId,
                        member.AddedAt
                    );

        return await query.ToListAsync(ct);
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
