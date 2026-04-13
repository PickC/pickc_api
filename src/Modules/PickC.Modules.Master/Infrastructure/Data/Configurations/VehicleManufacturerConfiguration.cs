using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class VehicleManufacturerConfiguration : IEntityTypeConfiguration<VehicleManufacturer>
{
    public void Configure(EntityTypeBuilder<VehicleManufacturer> builder)
    {
        builder.ToTable("VehicleManufacturer", "Master");

        builder.HasKey(v => v.ManufacturerId);
        builder.Property(v => v.ManufacturerId).ValueGeneratedOnAdd();
        builder.Property(v => v.Manufacturer).HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(v => v.MakeType).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(v => v.Capacity).HasColumnType("decimal(18,2)");
    }
}
