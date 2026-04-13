using Microsoft.EntityFrameworkCore;
using PickC.Modules.Trip.Domain.Entities;

namespace PickC.Modules.Trip.Infrastructure.Data;

public class TripDbContext : DbContext
{
    public TripDbContext(DbContextOptions<TripDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Trip> Trips => Set<Domain.Entities.Trip>();
    public DbSet<TripMonitor> TripMonitors => Set<TripMonitor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TripDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
