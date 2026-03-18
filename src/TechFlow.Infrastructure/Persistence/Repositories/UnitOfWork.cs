using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Infrastructure.Persistence.Repositories;

namespace TechFlow.Infrastructure.Persistence;
public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private IPermissionRepository? _permissions;
    private IRoleRepository?       _roles;
    private ICompanyRepository?    _companies;
    private IUserRepository?       _users;
    private IProjectRepository?    _projects;
    private IBoardRepository?      _boards;
    private ITaskRepository?       _tasks;

    // ── Properties 

    public IPermissionRepository Permissions =>
        _permissions ??= new PermissionRepository(context);

    public IRoleRepository Roles =>
        _roles ??= new RoleRepository(context);

    public ICompanyRepository Companies =>
        _companies ??= new CompanyRepository(context);
    public IUserRepository Users =>
        _users ??= new UserRepository(context);
    public IProjectRepository Projects =>
        _projects ??= new ProjectRepository(context);
    public IBoardRepository Boards =>
        _boards ??= new BoardRepository(context);

    public ITaskRepository Tasks => 
        _tasks ??= new TaskRepository(context);

    public ISprintRepository Sprints =>
        new SprintRepository(context);
    // ── Persistence 

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await context.SaveChangesAsync(ct);
}
