using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class VehicleConfigConfiguration : IEntityTypeConfiguration<VehicleConfig>
{
    public void Configure(EntityTypeBuilder<VehicleConfig> builder)
    {
        builder.ToTable("VehicleConfig", "Master");

        builder.HasKey(v => v.Model);
        builder.Property(v => v.Model).HasMaxLength(50);
        builder.Property(v => v.Maker).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Tonnage).HasColumnType("numeric(18,2)");
    }
}
