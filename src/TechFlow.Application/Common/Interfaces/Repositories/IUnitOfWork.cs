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
    IUserRepository Users { get; }
    IProjectRepository Projects { get; }
    IBoardRepository Boards { get; }
    ITaskRepository Tasks { get; }
    ISprintRepository Sprints { get; }
    IInvitationRepository Invitations { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}