using Claims.Data.Abstractions;
using Claims.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Data
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClaimsData(this IServiceCollection services)
        {
            services.AddScoped<IClaimsRepository, ClaimsRepository>();
            services.AddScoped<ICoversRepository, CoversRepository>();
            return services;
        }
    }
}
