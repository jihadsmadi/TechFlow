using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TechFlow.API.Services;
using TechFlow.Application.Common.Interfaces;

namespace TechFlow.API;

public static class DependencyInjection
{
    public static IServiceCollection AddAPI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ── Controllers ────────────────────────────────────────────────────────
        services.AddControllers();

        // ── OpenAPI ────────────────────────────────────────────────────────────
        services.AddOpenApi();

        // ── JWT Settings ───────────────────────────────────────────────────────
        //var jwtSettings = configuration
        //    .GetSection(JwtSettings.SectionName)
        //    .Get<JwtSettings>();

        //ArgumentNullException.ThrowIfNull(jwtSettings);
        //ArgumentException.ThrowIfNullOrWhiteSpace(jwtSettings.SecretKey);

        //services.Configure<JwtSettings>(
        //    configuration.GetSection(JwtSettings.SectionName));

        // ── JWT Authentication ─────────────────────────────────────────────────
        //services
        //    .AddAuthentication(options =>
        //    {
        //        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //    })
        //    .AddJwtBearer(options =>
        //    {
        //        options.TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidateAudience = true,
        //            ValidateLifetime = true,
        //            ValidateIssuerSigningKey = true,
        //            ValidIssuer = jwtSettings.Issuer,
        //            ValidAudience = jwtSettings.Audience,
        //            IssuerSigningKey = new SymmetricSecurityKey(
        //                Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        //            ClockSkew = TimeSpan.Zero
        //        };
        //    });

        services.AddAuthentication();
        services.AddAuthorization();

        // ── IUser ──────────────────────────────────────────────────────────────
        services.AddHttpContextAccessor();
        services.AddScoped<IUser, CurrentUser>();

        return services;
    }
}