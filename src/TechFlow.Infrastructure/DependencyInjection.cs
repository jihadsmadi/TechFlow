using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechFlow.Application.Common.Interfaces.Repositories;
using TechFlow.Application.Common.Interfaces.Services;
using TechFlow.Infrastructure.Identity;
using TechFlow.Infrastructure.Identity.Services;
using TechFlow.Infrastructure.Persistence;
using TechFlow.Infrastructure.Persistence.Interceptors;
using TechFlow.Infrastructure.Services;
using TechFlow.Infrastructure.Settings;

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

        // ── JWT Settings 
        services.Configure<JwtSettings>(
            configuration.GetSection(JwtSettings.SectionName));

        // ── Client Settings
        services.Configure<ClientSettings>(
            configuration.GetSection(ClientSettings.SectionName));

        // ── Interceptors 
        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        // ── DbContext 
        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options
                .UseSqlServer(
                    connectionString,
                    sql => sql.MigrationsAssembly(
                        typeof(ApplicationDbContext).Assembly.GetName().Name))
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

        // ── Repositories 
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Auth + Token Services 
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IEmailService, ConsoleEmailService>();
        services.AddScoped<IInvitationLinkBuilder, InvitationLinkBuilder>();
        // ── Seeder 
        services.AddScoped<ApplicationDbContextInitialiser>();

        // ── TimeProvider 
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}