using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Billing.Application.Services;
using PickC.Modules.Billing.Domain.Interfaces;
using PickC.Modules.Billing.Infrastructure.Data;
using PickC.Modules.Billing.Infrastructure.Repositories;

namespace PickC.Modules.Billing;

public static class BillingModule
{
    public static IServiceCollection AddBillingModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<BillingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("PickC"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Operation")));

        // Repositories
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        // Application Services
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IDriverBillingService, DriverBillingService>();

        // Validators
        services.AddValidatorsFromAssembly(typeof(BillingModule).Assembly);

        return services;
    }
}
