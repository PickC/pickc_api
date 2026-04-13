using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Infrastructure.Persistence;

namespace PickC.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
}
