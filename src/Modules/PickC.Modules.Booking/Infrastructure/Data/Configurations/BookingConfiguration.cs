using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Booking.Domain.Entities;

namespace PickC.Modules.Booking.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Domain.Entities.Booking>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Booking> builder)
    {
        builder.ToTable("Booking", "Operation");

        builder.HasKey(b => b.BookingNo);
        builder.Property(b => b.BookingNo).HasMaxLength(50);
        builder.Property(b => b.BookingDate).HasDefaultValueSql("getutcdate()");
        builder.Property(b => b.CustomerID).HasMaxLength(20).IsRequired();
        builder.Property(b => b.RequiredDate).HasDefaultValueSql("getutcdate()");
        builder.Property(b => b.LocationFrom).IsRequired();
        builder.Property(b => b.LocationTo).IsRequired();
        builder.Property(b => b.CargoDescription).HasMaxLength(100).IsRequired();
        builder.Property(b => b.VehicleType).IsRequired();
        builder.Property(b => b.Remarks).HasMaxLength(100).IsRequired();
        builder.Property(b => b.IsConfirm).HasDefaultValue(false);
        builder.Property(b => b.DriverID).HasMaxLength(20).HasDefaultValue("");
        builder.Property(b => b.VehicleNo).HasMaxLength(20).HasDefaultValue("");
        builder.Property(b => b.IsCancel).HasDefaultValue(false);
        builder.Property(b => b.CancelRemarks).HasMaxLength(100).HasDefaultValue("");
        builder.Property(b => b.IsComplete).HasDefaultValue(false);
        builder.Property(b => b.PayLoad).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(b => b.CargoType).HasMaxLength(100).IsRequired();
        builder.Property(b => b.Latitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(b => b.Longitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(b => b.ToLatitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(b => b.ToLongitude).HasColumnType("decimal(18,10)").HasDefaultValue(0m);
        builder.Property(b => b.IsReachPickUp).HasDefaultValue(false);
        builder.Property(b => b.IsReachDestination).HasDefaultValue(false);
        builder.Property(b => b.ReceiverMobileNo).HasMaxLength(15).IsRequired();
        builder.Property(b => b.IsCancelByDriver).HasDefaultValue(false);
        builder.Property(b => b.DriverCancelRemarks).HasMaxLength(100).HasDefaultValue("");
        builder.Property(b => b.LoadingUnLoading).HasDefaultValue((short)0);
        builder.Property(b => b.Status).IsRequired();
        builder.Property(b => b.OTP).HasMaxLength(10);
    }
}
