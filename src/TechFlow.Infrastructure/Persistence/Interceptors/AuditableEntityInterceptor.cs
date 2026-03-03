using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Domain.Common;

namespace TechFlow.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically stamps CreatedBy/CreatedAtUtc/LastModifiedBy/LastModifiedUtc
/// on every AuditableEntity before SaveChanges fires.
/// Injected into DbContext via AddDbContext options.
/// </summary>
public sealed class AuditableEntityInterceptor(
    IUser user,
    TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    // to do: add wait?
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null) return;

        var userId = user.Id?.ToString();
        var utcNow = timeProvider.GetUtcNow();

        foreach (var entry in context.ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified
                || entry.HasChangedOwnedEntities())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy    = userId;
                    entry.Entity.CreatedAtUtc = utcNow;
                }

                entry.Entity.LastModifiedBy  = userId;
                entry.Entity.LastModifiedUtc = utcNow;
            }
        }
    }
}

public static class EntityEntryExtensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry?.Metadata.IsOwned() == true &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
}
