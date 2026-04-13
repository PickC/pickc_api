using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.ToTable("Vehicle", "Master");

        builder.HasKey(v => v.VehicleNo);
        builder.Property(v => v.VehicleNo).HasMaxLength(20);
        builder.Property(v => v.OperatorID).HasMaxLength(20);
    }
}
