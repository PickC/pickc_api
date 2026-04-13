using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> builder)
    {
        builder.ToTable("Driver", "Master");

        builder.HasKey(d => d.DriverID);
        builder.Property(d => d.DriverID).HasMaxLength(50);
        builder.Property(d => d.DriverName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Password).HasMaxLength(50).IsRequired();
        builder.Property(d => d.VehicleNo).HasMaxLength(15).IsRequired();
        builder.Property(d => d.FatherName).HasMaxLength(100).IsRequired();
        builder.Property(d => d.PlaceOfBirth).HasMaxLength(100).IsRequired();
        builder.Property(d => d.MobileNo).HasMaxLength(10).IsRequired();
        builder.Property(d => d.PhoneNo).HasMaxLength(20);
        builder.Property(d => d.PANNo).HasMaxLength(10);
        builder.Property(d => d.AadharCardNo).HasMaxLength(50);
        builder.Property(d => d.LicenseNo).HasMaxLength(50).IsRequired();
        builder.Property(d => d.CreatedBy).HasMaxLength(50).IsRequired();
        builder.Property(d => d.ModifiedBy).HasMaxLength(50).IsRequired();
        builder.Property(d => d.VerifiedBy).HasMaxLength(50).IsRequired();
        builder.Property(d => d.DeviceID).HasMaxLength(200).IsRequired();
        builder.Property(d => d.Nationality).HasMaxLength(50).IsUnicode(false);
        builder.Property(d => d.OperatorID).HasMaxLength(50);
        builder.Property(d => d.MobileMake).HasMaxLength(20).IsUnicode(false);
        builder.Property(d => d.ModelNo).HasMaxLength(20).IsUnicode(false);
        builder.Property(d => d.DeviceRemarks).HasMaxLength(1000).IsUnicode(false);
        builder.Property(d => d.VehicleRCNo).HasMaxLength(50);

        builder.HasMany(d => d.Addresses)
            .WithOne()
            .HasForeignKey(a => a.AddressLinkID)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(d => d.Attachments)
            .WithOne()
            .HasForeignKey(a => a.DriverID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.BankDetails)
            .WithOne()
            .HasForeignKey<DriverBankDetails>(b => b.DriverID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Ratings)
            .WithOne()
            .HasForeignKey(r => r.DriverID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Referrals)
            .WithOne()
            .HasForeignKey(r => r.DriverID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Operator)
            .WithMany(o => o.Drivers)
            .HasForeignKey(d => d.OperatorID)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
