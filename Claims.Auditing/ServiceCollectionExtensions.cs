using Claims.Auditing.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Auditing
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClaimsAuditing(this IServiceCollection services)
        {
            services.AddScoped<IAuditer, Auditer>();
            services.AddSingleton<IAuditQueue, AuditQueue>();
            services.AddHostedService<AuditBackgroundService>();
            return services;
        }
    }
}
