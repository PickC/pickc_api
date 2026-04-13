using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Master.Application.Services;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.Modules.Master.Infrastructure.Data;
using PickC.Modules.Master.Infrastructure.Repositories;

namespace PickC.Modules.Master;

public static class MasterModule
{
    public static IServiceCollection AddMasterModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<MasterDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PickC"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Master")));

        // Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IDriverRepository, DriverRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<ILookUpRepository, LookUpRepository>();

        // Application Services
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IDriverService, DriverService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<ILookUpService, LookUpService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(MasterModule).Assembly);

        return services;
    }
}
