namespace TechFlow.Application.Common.Interfaces.Repositories;

/// <summary>
/// Unit of Work — single entry point for all repositories.
/// One call to SaveChangesAsync commits everything atomically.
/// </summary>
public interface IUnitOfWork
{
    IPermissionRepository Permissions { get; }
    IRoleRepository Roles { get; }
    ICompanyRepository Companies { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}