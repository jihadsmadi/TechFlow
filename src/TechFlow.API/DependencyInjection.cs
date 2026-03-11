using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TechFlow.API.Authorization;
using TechFlow.API.Services;
using TechFlow.Application.Common.Interfaces;
using TechFlow.Infrastructure.Settings;

namespace TechFlow.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Controllers 
        services.AddControllers();

        services.AddOpenApi();


        var jwtSettings = configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>();

        ArgumentNullException.ThrowIfNull(jwtSettings,
            "JwtSettings section is missing from configuration.");
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtSettings.SecretKey,
            "JwtSettings:SecretKey must not be empty.");

        // ── JWT Authentication 
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                    ClockSkew = TimeSpan.Zero  
                };
            });

        services.AddAuthorization();

        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

        // ── Current User 
        services.AddHttpContextAccessor();
        services.AddScoped<IUser, CurrentUser>();

        return services;
    }
}