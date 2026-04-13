using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Identity.Domain.Entities;

namespace PickC.Modules.Identity.Infrastructure.Data.Configurations;

public class DriverActivityConfiguration : IEntityTypeConfiguration<DriverActivity>
{
    public void Configure(EntityTypeBuilder<DriverActivity> builder)
    {
        builder.ToTable("DriverActivity", "Operation");

        builder.HasKey(d => new { d.TokenNo, d.DriverId });
        builder.Property(d => d.DriverId).HasColumnName("DriverID").HasMaxLength(50);
        builder.Property(d => d.VehicleNo).HasMaxLength(20);
        builder.Property(d => d.Latitude).HasColumnType("decimal(18,10)");
        builder.Property(d => d.Longitude).HasColumnType("decimal(18,10)");
        builder.Property(d => d.CurrentLat).HasColumnType("decimal(18,10)");
        builder.Property(d => d.CurrentLong).HasColumnType("decimal(18,10)");
        builder.Property(d => d.LogOutLat).HasColumnType("decimal(18,10)");
        builder.Property(d => d.LogOutLong).HasColumnType("decimal(18,10)");
    }
}
