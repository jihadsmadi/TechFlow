using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechFlow.Application.Common.Events;
using TechFlow.Domain.Boards;
using TechFlow.Domain.Common;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Projects;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Sprints;
using TechFlow.Domain.Sprints.ValueObjects;
using TechFlow.Domain.Tasks.Subtasks;
using TechFlow.Domain.Tasks.TaskAssignments;
using TechFlow.Domain.Users;
using TechFlow.Domain.Users.UserCompanyRoles;
using TechFlow.Domain.Users.UserProjectRoles;
using TechFlow.Infrastructure.Identity;

using DomainTask = TechFlow.Domain.Tasks.Task;
namespace TechFlow.Infrastructure.Persistence;

public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    private readonly IMediator _mediator = mediator;

    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<UserCompanyRole> UserCompanyRoles => Set<UserCompanyRole>();
    public DbSet<UserProjectRole> UserProjectRoles => Set<UserProjectRole>();

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    public DbSet<Board> Boards => Set<Board>();
    public DbSet<List> Lists => Set<List>();

    public DbSet<DomainTask> Tasks => Set<DomainTask>();
    public DbSet<Subtask> Subtasks => Set<Subtask>();
    public DbSet<TaskAssignment> TaskAssignments => Set<TaskAssignment>();

    public DbSet<Sprint> Sprints => Set<Sprint>();
    public DbSet<SprintItem> SprintItems => Set<SprintItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        builder.Ignore<Domain.Tasks.Attachments.Attachment>();
        builder.Ignore<Domain.Tasks.CustomeFields.CustomFieldDefinition>();
        builder.Ignore<Domain.Tasks.CustomeFields.CustomFieldValue>();
        builder.Ignore<Domain.Tasks.Comments.Comment>();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var result = await base.SaveChangesAsync(ct);
        await DispatchDomainEventsAsync(ct);

        return result;
    }

    private async Task DispatchDomainEventsAsync(CancellationToken ct)
    {
        var domainEntities = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        domainEntities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            var notificationType = typeof(DomainEventNotification<>)
                .MakeGenericType(domainEvent.GetType());

            var notification = Activator.CreateInstance(notificationType, domainEvent)!;

            await _mediator.Publish(notification, ct);
        }
    }
}