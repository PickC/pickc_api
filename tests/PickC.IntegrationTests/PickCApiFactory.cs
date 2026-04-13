using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PickC.Modules.Billing.Infrastructure.Data;
using PickC.Modules.Booking.Infrastructure.Data;
using PickC.Modules.Identity.Infrastructure.Data;
using PickC.Modules.Master.Infrastructure.Data;
using PickC.Modules.Trip.Infrastructure.Data;

namespace PickC.IntegrationTests;

public class PickCApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");

        builder.ConfigureTestServices(services =>
        {
            // Remove ALL EF Core / database provider service registrations
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType.FullName != null &&
                    (d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                     d.ServiceType.FullName.Contains("DbContextOptions") ||
                     d.ImplementationType?.FullName?.Contains("SqlServer") == true))
                .ToList();

            foreach (var d in descriptorsToRemove)
                services.Remove(d);

            // Also remove the DbContext types themselves
            RemoveService<MasterDbContext>(services);
            RemoveService<BookingDbContext>(services);
            RemoveService<TripDbContext>(services);
            RemoveService<BillingDbContext>(services);
            RemoveService<IdentityDbContext>(services);

            // Re-register all DbContexts with InMemory provider
            services.AddDbContext<MasterDbContext>(options =>
                options.UseInMemoryDatabase("MasterTestDb"));
            services.AddDbContext<BookingDbContext>(options =>
                options.UseInMemoryDatabase("BookingTestDb"));
            services.AddDbContext<TripDbContext>(options =>
                options.UseInMemoryDatabase("TripTestDb"));
            services.AddDbContext<BillingDbContext>(options =>
                options.UseInMemoryDatabase("BillingTestDb"));
            services.AddDbContext<IdentityDbContext>(options =>
                options.UseInMemoryDatabase("IdentityTestDb"));
        });
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }
}
