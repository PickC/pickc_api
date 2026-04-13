using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Identity.Domain.Entities;

namespace PickC.Modules.Identity.Infrastructure.Data.Configurations;

public class DriverCredentialConfiguration : IEntityTypeConfiguration<DriverCredential>
{
    public void Configure(EntityTypeBuilder<DriverCredential> builder)
    {
        builder.ToTable("Driver", "Master");

        builder.HasKey(d => d.DriverId);
        builder.Property(d => d.DriverId).HasColumnName("DriverID").HasMaxLength(50);
        builder.Property(d => d.Password).HasMaxLength(50).IsRequired();
        builder.Property(d => d.MobileNo).HasMaxLength(10).IsRequired();
        builder.Property(d => d.VehicleNo).HasMaxLength(15).IsRequired();
        builder.Property(d => d.Status).IsRequired();
    }
}
