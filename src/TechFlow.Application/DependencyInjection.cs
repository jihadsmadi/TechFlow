using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TechFlow.Application.Common.Behaviours;

namespace TechFlow.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>)); // outermost
            //cfg.AddOpenBehavior(typeof(LoggingBehaviour<,>));
            //cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CacheInvalidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));            // innermost
        });

        // ── FluentValidation 
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // ── HybridCache 
        services.AddHybridCache();

        return services;
    }
}
