using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class DriverRatingConfiguration : IEntityTypeConfiguration<DriverRating>
{
    public void Configure(EntityTypeBuilder<DriverRating> builder)
    {
        builder.ToTable("DriverRating", "Master");

        builder.HasKey(r => r.BookingNo);
        builder.Property(r => r.BookingNo).HasMaxLength(50);
        builder.Property(r => r.DriverID).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Remarks).HasColumnType("nvarchar(max)");
    }
}
