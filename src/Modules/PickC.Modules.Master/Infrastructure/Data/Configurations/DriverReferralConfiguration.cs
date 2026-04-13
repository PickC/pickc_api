using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class DriverReferralConfiguration : IEntityTypeConfiguration<DriverReferral>
{
    public void Configure(EntityTypeBuilder<DriverReferral> builder)
    {
        builder.ToTable("DriverReferral", "Master");

        builder.HasKey(r => r.ReferralId);
        builder.Property(r => r.ReferralId).ValueGeneratedOnAdd();
        builder.Property(r => r.DriverID).HasMaxLength(50).IsRequired();
        builder.Property(r => r.Name).HasMaxLength(100).IsRequired();
        builder.Property(r => r.Mobile).HasMaxLength(30).IsRequired();
        builder.Property(r => r.EmailID).HasMaxLength(100);
        builder.Property(r => r.ReferalAmount).HasColumnType("decimal(18,2)");
        builder.Property(r => r.ReferalAmountPaid).HasColumnType("decimal(18,2)");
        builder.Property(r => r.CreatedBy).HasMaxLength(50);
        builder.Property(r => r.Modifiedby).HasMaxLength(100);
    }
}
