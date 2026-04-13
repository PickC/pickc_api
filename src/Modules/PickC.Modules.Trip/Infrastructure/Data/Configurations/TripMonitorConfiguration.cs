using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Trip.Domain.Entities;

namespace PickC.Modules.Trip.Infrastructure.Data.Configurations;

public class TripMonitorConfiguration : IEntityTypeConfiguration<TripMonitor>
{
    public void Configure(EntityTypeBuilder<TripMonitor> builder)
    {
        builder.ToTable("TripMonitor", "Operation");

        builder.HasKey(t => new { t.DriverID, t.TripID });
        builder.Property(t => t.DriverID).HasMaxLength(50);
        builder.Property(t => t.TripID).HasMaxLength(50);
        builder.Property(t => t.VehicleNo).HasMaxLength(15).IsRequired();
        builder.Property(t => t.RefreshDate).HasDefaultValueSql("getdate()");
        builder.Property(t => t.Latitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(t => t.Longitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(t => t.TripType).HasDefaultValue((short)0);
        builder.Property(t => t.Bearing).HasColumnType("decimal(5,2)").IsRequired(false);
        builder.Property(t => t.SpeedKmh).HasColumnType("decimal(5,2)").IsRequired(false);
    }
}
