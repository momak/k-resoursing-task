using Claims.Services.Abstractions;
using Claims.Services.Services;
using Claims.Services.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Claims.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClaimsServices(this IServiceCollection services)
        {
            services.AddScoped<IPremiumCalculator, PremiumCalculator>();
            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<ICoversService, CoversService>();
            services.AddValidatorsFromAssemblyContaining<CoverValidator>();
            return services;
        }
    }
}
        