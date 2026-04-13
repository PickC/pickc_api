using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class OperatorConfiguration : IEntityTypeConfiguration<Operator>
{
    public void Configure(EntityTypeBuilder<Operator> builder)
    {
        builder.ToTable("Operator", "Master");

        builder.HasKey(o => o.OperatorID);
        builder.Property(o => o.OperatorID).HasMaxLength(50);
        builder.Property(o => o.OperatorName).HasMaxLength(100).IsRequired();
        builder.Property(o => o.Password).HasMaxLength(50).IsRequired();
        builder.Property(o => o.FatherName).HasMaxLength(100).IsRequired();
        builder.Property(o => o.PlaceOfBirth).HasMaxLength(50).IsRequired();
        builder.Property(o => o.MobileNo).HasMaxLength(20).IsRequired();
        builder.Property(o => o.PhoneNo).HasMaxLength(20);
        builder.Property(o => o.PANNo).HasMaxLength(20);
        builder.Property(o => o.AadharCardNo).HasMaxLength(20);
        builder.Property(o => o.Nationality).HasMaxLength(50).IsRequired();
        builder.Property(o => o.VerifiedBy).HasMaxLength(50);
        builder.Property(o => o.CreatedBy).HasMaxLength(50);
        builder.Property(o => o.ModifiedBy).HasMaxLength(50);
    }
}
