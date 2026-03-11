using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechFlow.Domain.Companies;
using TechFlow.Domain.Permissions;
using TechFlow.Domain.Permissions.Const;
using TechFlow.Domain.Roles;
using TechFlow.Domain.Users;
using TechFlow.Domain.Users.UserCompanyRoles;
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
    // ── Migrations ────────────────────────────────────────────────────────────

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

    // ── Seeding ───────────────────────────────────────────────────────────────

    public async Task SeedAsync()
    {
        try
        {
            await SeedPermissionsAsync();
            await SeedRolesAsync();
            await SeedRolePermissionsAsync(); 
            await SeedAdminUserAsync();

            logger.LogInformation("Database seeded successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    // ── Seed Permissions ──────────────────────────────────────────────────────

    private async Task SeedPermissionsAsync()
    {
        var allNames = PermissionNames.All;

        foreach (var name in allNames)
        {
            var permissions = await context.Permissions.ToListAsync();
            var exists = permissions.Any(p => p.Name == name);
            if (exists) continue;

            if(name == PermissionNames.PermissionsCreate)
            {
                Console.Write("");
            }
            var group = name.Split('.')[0];
            var groupFormatted = char.ToUpper(group[0]) + group[1..];

            var result = Permission.Create(
                name,
                groupFormatted,
                $"Allows {name.Replace('.', ' ')} operation.");

            if (result.IsFailure)
            {
                logger.LogWarning("Failed to seed permission {Name}: {Error}",
                    name, result.TopError);
                continue;
            }

            context.Permissions.Add(result.Value);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Permissions seeded.");
    }

    // ── Seed Roles ────────────────────────────────────────────────────────────

    private async Task SeedRolesAsync()
    {
        foreach (var (roleName, roleDescription) in SystemRoles.All)
        {
            var exists = await context.Roles.AnyAsync(r => r.Name == roleName);
            if (exists) continue;

            var newRole = Role.SodoCreateSystem(Guid.NewGuid(), roleName, roleDescription);
            context.Roles.Add(newRole);
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Roles seeded.");
    }

    // ── Seed Role Permissions ─────────────────────────────────────────────────

    private async Task SeedRolePermissionsAsync()
    {
        var allPermissions = await context.Permissions.ToListAsync();

        var permissionByName = allPermissions.ToDictionary(p => p.Name);

        // ── Admin gets everything ─────────────────────────────────────────────
        await AssignPermissionsToRoleAsync(
            SystemRoles.Admin,
            PermissionNames.All,
            permissionByName);

        // ── ProjectManager ────────────────────────────────────────────────────
        await AssignPermissionsToRoleAsync(
            SystemRoles.ProjectManager,
            [
                PermissionNames.ProjectsRead,
                PermissionNames.ProjectsCreate,
                PermissionNames.ProjectsUpdate,
                PermissionNames.ProjectsArchive,
                PermissionNames.ProjectsManageMembers,
                PermissionNames.TasksRead,
                PermissionNames.TasksCreate,
                PermissionNames.TasksUpdate,
                PermissionNames.TasksDelete,
                PermissionNames.TasksAssign,
                PermissionNames.TasksMove,
                PermissionNames.UsersRead,
            ],
            permissionByName);

        // ── Developer ─────────────────────────────────────────────────────────
        await AssignPermissionsToRoleAsync(
            SystemRoles.Developer,
            [
                PermissionNames.ProjectsRead,
                PermissionNames.TasksRead,
                PermissionNames.TasksCreate,
                PermissionNames.TasksUpdate,
                PermissionNames.TasksMove,
            ],
            permissionByName);

        // ── Intern ────────────────────────────────────────────────────────────
        await AssignPermissionsToRoleAsync(
            SystemRoles.Intern,
            [
                PermissionNames.ProjectsRead,
                PermissionNames.TasksRead,
            ],
            permissionByName);

        await context.SaveChangesAsync();
        logger.LogInformation("Role permissions seeded.");
    }

    private async Task AssignPermissionsToRoleAsync(
        string roleName,
        IEnumerable<string> permissionNames,
        Dictionary<string, Permission> permissionByName)
    {
        var role = await context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Name == roleName);

        if (role is null)
        {
            logger.LogWarning("Role {RoleName} not found during permission seeding.", roleName);
            return;
        }

        foreach (var name in permissionNames)
        {
            if (!permissionByName.TryGetValue(name, out var permission))
            {
                logger.LogWarning("Permission {Name} not found during role seeding.", name);
                continue;
            }

            // skip if already assigned
            if (role.Permissions.Any(p => p.Id == permission.Id))
                continue;

            role.GrantPermission(permission);
        }
    }

    // ── Seed Admin User ───────────────────────────────────────────────────────

    private async Task SeedAdminUserAsync()
    {
        const string adminEmail = "admin@techflow.dev";
        const string adminPassword = "Admin@123456";

        // ── skip if already seeded ────────────────────────────────────────────
        var existing = await userManager.FindByEmailAsync(adminEmail);
        if (existing is not null) return;

        // ── 1. seed a system company for the admin ────────────────────────────
        var company = await context.Companies
            .FirstOrDefaultAsync(c => c.Slug.Value == "techflow-system");

        if (company is null)
        {
            var companyResult = Company.Create(
                "TechFlow System",
                "techflow-system",
                "system@techflow.dev",
                "Technology");

            if (companyResult.IsFailure)
            {
                logger.LogWarning("Failed to seed system company: {Error}",
                    companyResult.TopError);
                return;
            }

            company = companyResult.Value;
            context.Companies.Add(company);
            await context.SaveChangesAsync();
        }

        // ── 2. create AppUser (Identity) ──────────────────────────────────────
        var appUserId = Guid.NewGuid();
        var appUser = new AppUser
        {
            Id = appUserId,
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true,
        };

        var identityResult = await userManager.CreateAsync(appUser, adminPassword);
        if (!identityResult.Succeeded)
        {
            logger.LogWarning("Failed to seed admin AppUser: {Errors}",
                string.Join(", ", identityResult.Errors.Select(e => e.Description)));
            return;
        }

        // ── 3. create domain User ─────────────────────────────────────────────
        var userResult = User.Create(
            appUserId,
            company.Id,
            "System",
            "Admin",
            adminEmail);

        if (userResult.IsFailure)
        {
            logger.LogWarning("Failed to seed admin domain User: {Error}",
                userResult.TopError);
            return;
        }

        var domainUser = userResult.Value;

        /// ── 4. assign Admin role company-wide ─────────────────────────────────
        var adminRole = await context.Roles
            .FirstOrDefaultAsync(r => r.Name == SystemRoles.Admin);

        if (adminRole is null)
        {
            logger.LogWarning("Admin role not found — cannot assign to seeded admin user.");
            return;
        }

        // CREATE the role assignment — don't query for it, it doesn't exist yet
        var companyRoleResult = UserCompanyRole.Create(
            userId: domainUser.Id,
            roleId: adminRole.Id,
            assignedByUserId: appUserId);   // self-assigned during seeding

        if (companyRoleResult.IsFailure)
        {
            logger.LogWarning("Failed to create admin company role: {Error}",
                companyRoleResult.TopError);
            return;
        }

        domainUser.AssignCompanyRole(companyRoleResult.Value);

        context.Users.Add(domainUser);
        await context.SaveChangesAsync();

        logger.LogInformation("Admin user seeded: {Email}", adminEmail);
    }
}