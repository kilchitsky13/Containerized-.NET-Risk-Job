using Microsoft.Extensions.DependencyInjection;
using Risk_Job.Domain.Interfaces;
using Risk_Job.Infrastructure.Repositories;

namespace Risk_Job.Infrastructure.Configurations;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<IBorrowerRiskRepository, InMemoryBorrowerRiskRepository>();

        return services;
    }
}
