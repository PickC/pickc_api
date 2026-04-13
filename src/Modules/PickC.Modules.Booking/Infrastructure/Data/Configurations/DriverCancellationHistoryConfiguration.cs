using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Booking.Domain.Entities;

namespace PickC.Modules.Booking.Infrastructure.Data.Configurations;

public class DriverCancellationHistoryConfiguration : IEntityTypeConfiguration<DriverCancellationHistory>
{
    public void Configure(EntityTypeBuilder<DriverCancellationHistory> builder)
    {
        builder.ToTable("DriverCancellationHistory", "Operation");

        builder.HasKey(d => new { d.DriverID, d.BookingNo });
        builder.Property(d => d.DriverID).HasMaxLength(20);
        builder.Property(d => d.BookingNo).HasMaxLength(50);
        builder.Property(d => d.CancelRemarks).HasMaxLength(100).IsRequired();
    }
}
