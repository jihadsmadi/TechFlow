using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Infrastructure.Persistence.Repositories;

namespace TechFlow.Infrastructure.Persistence;
public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IPermissionRepository? _permissions;
    private IRoleRepository?       _roles;

    private ICompanyRepository?    _companies;

    // ── Properties 

    public IPermissionRepository Permissions =>
        _permissions ??= new PermissionRepository(context);

    public IRoleRepository Roles =>
        _roles ??= new RoleRepository(context);

    public ICompanyRepository Companies =>
        _companies ??= new CompanyRepository(context);

    // ── Persistence 

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
