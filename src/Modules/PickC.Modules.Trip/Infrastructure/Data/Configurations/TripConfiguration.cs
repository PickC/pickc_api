using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PickC.Modules.Trip.Infrastructure.Data.Configurations;

public class TripConfiguration : IEntityTypeConfiguration<Domain.Entities.Trip>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Trip> builder)
    {
        builder.ToTable("Trip", "Operation");

        builder.HasKey(t => t.TripID);
        builder.Property(t => t.TripID).HasMaxLength(50);
        builder.Property(t => t.TripDate).HasDefaultValueSql("getutcdate()");
        builder.Property(t => t.CustomerMobile).HasMaxLength(20).IsRequired();
        builder.Property(t => t.DriverID).HasMaxLength(20).IsRequired();
        builder.Property(t => t.VehicleNo).HasMaxLength(20).IsRequired();
        builder.Property(t => t.VehicleType).HasDefaultValue((short)0);
        builder.Property(t => t.VehicleGroup).HasDefaultValue((short)0);
        builder.Property(t => t.LocationFrom);
        builder.Property(t => t.LocationTo);
        builder.Property(t => t.Distance).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(t => t.StartTime).HasDefaultValueSql("getutcdate()");
        builder.Property(t => t.TripMinutes).HasColumnType("numeric(18,0)").HasDefaultValue(0m);
        builder.Property(t => t.WaitingMinutes).HasColumnType("numeric(18,0)").HasDefaultValue(0m);
        builder.Property(t => t.TotalWeight).HasMaxLength(10).IsUnicode(false).IsRequired();
        builder.Property(t => t.CargoDescription).HasMaxLength(100);
        builder.Property(t => t.Remarks).HasMaxLength(100);
        builder.Property(t => t.Latitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(t => t.Longitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(t => t.TripEndLat).HasColumnType("numeric(18,10)");
        builder.Property(t => t.TripEndLong).HasColumnType("numeric(18,10)");
        builder.Property(t => t.DistanceTravelled).HasColumnType("numeric(18,2)");
        builder.Property(t => t.BookingNo).HasMaxLength(50);
    }
}
