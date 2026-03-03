using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Permissions.Const;
using TechFlow.Domain.Roles;
using TechFlow.Infrastructure.Identity;

namespace TechFlow.Infrastructure.Persistence;

/// <summary>
/// Runs on app startup — applies migrations and seeds required data.
/// Call InitialiseAsync() then SeedAsync() from Program.cs.
/// </summary>
public sealed class ApplicationDbContextInitialiser(
    ApplicationDbContext context,
    UserManager<AppUser> userManager,
    ILogger<ApplicationDbContextInitialiser> logger)
{
    // ── Migrations 

    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrated successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }

    // ── Seeding 

    public async Task SeedAsync()
    {
        try
        {
            await SeedPermissionsAsync();
            await SeedRolesAsync();
            await SeedAdminUserAsync();

            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    // ── Seed Permissions 

    private async Task SeedPermissionsAsync()
    {
        // All permission names from PermissionNames constants
        var allNames = PermissionNames.All;

        foreach (var name in allNames)
        {
            var exists = await context.Permissions
                .AnyAsync(p => p.Name == name);

            if (exists) continue;

            // Extract group from name (e.g. "tasks.create" → "tasks")
            var group = name.Split('.')[0];
            var groupFormatted = char.ToUpper(group[0]) + group[1..];

            var result = Permission.Create(
                name,
                groupFormatted,
                $"Allows {name.Replace('.', ' ')} operation.");

            if (result.IsFailure)
            {
                logger.LogWarning("Failed to seed permission {Name}: {Error}", name, result.TopError);
                continue;
            }

            context.Permissions.Add(result.Value);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Permissions seeded.");
    }

    // ── Seed Roles 

    private async Task SeedRolesAsync()
    {
        // System roles defined in SystemRoles — protected from modification
        var systemRoles = SystemRoles.All;

        foreach (var (roleName,roleDescription) in systemRoles)
        {
            var exists = await context.Roles.AnyAsync(r => r.Name == roleName);
            if (exists) continue;

            var newRole = Role.SodoCreateSystem(Guid.NewGuid(), roleName, roleDescription);
            context.Roles.Add(newRole);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Roles seeded.");
    }

    // ── Seed Admin User 

    private async Task SeedAdminUserAsync()
    {
        const string adminEmail    = "admin@techflow.dev";
        const string adminPassword = "Admin@123456";

        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null) return;

        var appUser = new AppUser
        {
            Id            = Guid.NewGuid(),
            UserName      = adminEmail,
            Email         = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(appUser, adminPassword);

        if (!result.Succeeded)
        {
            logger.LogWarning(
                "Failed to seed admin user: {Errors}",
                string.Join(", ", result.Errors.Select(e => e.Description)));
            return;
        }

        logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }
}
