using Microsoft.EntityFrameworkCore;
using PickC.Modules.Booking.Domain.Entities;

namespace PickC.Modules.Booking.Infrastructure.Data;

public class BookingDbContext : DbContext
{
    public BookingDbContext(DbContextOptions<BookingDbContext> options) : base(options) { }

    public DbSet<Domain.Entities.Booking> Bookings => Set<Domain.Entities.Booking>();
    public DbSet<DriverCancellationHistory> DriverCancellationHistories => Set<DriverCancellationHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
