using TechFlow.Application.Features.Projects.Dtos;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Projects;

namespace TechFlow.Application.Common.Interfaces.Repositories;

public interface IProjectRepository : IRepository<Project>
{
    Task<Project?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
    Task<List<Project>> GetByCompanyAsync(
        Guid companyId,
        Guid? userId,
        bool includeArchived,
        CancellationToken ct = default);
    Task<List<ProjectMemberWithUserDto>> GetMembersWithUserDetailsAsync(Guid projectId, CancellationToken ct);
    Task<List<Project>> GetByMemberAsync(Guid userId, CancellationToken ct = default);
    Task<bool> NameExistsInCompanyAsync(Guid companyId, string name, Guid? excludeId = null, CancellationToken ct = default);
    Task<bool> IsMemberAsync(Guid projectId, Guid userId, CancellationToken ct = default);
}
