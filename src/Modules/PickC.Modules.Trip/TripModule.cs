using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Trip.Application.Services;
using PickC.Modules.Trip.Domain.Interfaces;
using PickC.Modules.Trip.Infrastructure.Data;
using PickC.Modules.Trip.Infrastructure.Repositories;

namespace PickC.Modules.Trip;

public static class TripModule
{
    public static IServiceCollection AddTripModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<TripDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PickC"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Operation")));

        // Repositories
        services.AddScoped<ITripRepository, TripRepository>();
        services.AddScoped<ITripMonitorRepository, TripMonitorRepository>();

        // Application Services
        services.AddScoped<ITripService, TripService>();
        services.AddScoped<ITripMonitorService, TripMonitorService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(TripModule).Assembly);

        return services;
    }
}
