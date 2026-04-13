using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class RateCardConfiguration : IEntityTypeConfiguration<RateCard>
{
    public void Configure(EntityTypeBuilder<RateCard> builder)
    {
        builder.ToTable("RateCard", "Master");

        builder.HasKey(r => new { r.Category, r.VehicleType, r.RateType });
        builder.Property(r => r.BaseFare).HasColumnType("numeric(18,2)");
        builder.Property(r => r.BaseKM).HasColumnType("numeric(18,2)");
        builder.Property(r => r.DistanceFare).HasColumnType("numeric(18,2)");
        builder.Property(r => r.RideTimeFare).HasColumnType("numeric(18,2)");
        builder.Property(r => r.WaitingFare).HasColumnType("numeric(18,2)");
        builder.Property(r => r.CancellationFee).HasColumnType("numeric(18,2)");
        builder.Property(r => r.DriverAssistance).HasColumnType("numeric(18,2)");
        builder.Property(r => r.OverNightCharges).HasColumnType("numeric(18,2)");
    }
}
