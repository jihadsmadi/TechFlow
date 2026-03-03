using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechFlow.Domain.Common;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Users;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Boards;
using TechFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace TechFlow.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator) : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)

{
    private readonly IMediator _mediator = mediator;

    // ── Domain DbSets 

    public DbSet<Permission>    Permissions    => Set<Permission>();
    public DbSet<Role>          Roles          => Set<Role>();
    public DbSet<Company>       Companies      => Set<Company>();
    public DbSet<User>          Users          => Set<User>();
    public DbSet<Project>       Projects       => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<Board>         Boards         => Set<Board>();
    public DbSet<List>          Lists          => Set<List>();
    public DbSet<Domain.Tasks.Task> Tasks => Set<Domain.Tasks.Task>();

    // ── EF Core Configuration 

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // ── Ignore entities until their configs are built ──────────────────────
        builder.Ignore<TechFlow.Domain.Projects.Project>();
        builder.Ignore<TechFlow.Domain.Projects.ProjectMember>();
        builder.Ignore<TechFlow.Domain.Projects.ProjectSettings.ProjectSetting>();
        builder.Ignore<TechFlow.Domain.Projects.ValueObjects.ProjectColor>();
        builder.Ignore<TechFlow.Domain.Boards.Board>();
        builder.Ignore<TechFlow.Domain.Boards.List>();
        builder.Ignore<TechFlow.Domain.Users.User>();
        builder.Ignore<TechFlow.Domain.Users.UserRoles.UserRole>();
        builder.Ignore<TechFlow.Domain.Users.UserPreferences>();
        builder.Ignore<TechFlow.Domain.Tasks.Task>();
    }


    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);

        await DispatchDomainEventsAsync(ct);

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        //var entities = ChangeTracker
        //    .Entries<Entity>()
        //    .Where(e => e.Entity.DomainEvents.Any())
        //    .Select(e => e.Entity)
        //    .ToList();

        //var events = entities
        //    .SelectMany(e => e.DomainEvents)
        //    .ToList();

        //entities.ForEach(e => e.ClearDomainEvents());

        //foreach (var domainEvent in events)
        //    await _mediator.Publish(domainEvent, ct);
    }
}
