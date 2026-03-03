using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Infrastructure.Identity;
using TechFlow.Infrastructure.Persistence;
using TechFlow.Infrastructure.Persistence.Interceptors;

namespace TechFlow.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Connection String 
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        ArgumentNullException.ThrowIfNull(connectionString);

        // ── Interceptors 
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        // ── DbContext 
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options
                .UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name))
                .AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
        });

        // ── Identity 
        services.AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.User.RequireUniqueEmail = true;
        })
        .AddRoles<IdentityRole<Guid>>()
        .AddEntityFrameworkStores<ApplicationDbContext>();
        //.AddDefaultTokenProviders();

        // ── Repository + UnitOfWork 
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Seeder 
        services.AddScoped<ApplicationDbContextInitialiser>();

        // ── TimeProvider 
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}