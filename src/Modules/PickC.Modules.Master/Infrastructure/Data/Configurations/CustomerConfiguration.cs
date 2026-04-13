using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customer", "Master");

        builder.HasKey(c => c.MobileNo);
        builder.Property(c => c.MobileNo).HasMaxLength(20);
        builder.Property(c => c.Password).HasMaxLength(15).IsRequired();
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();
        builder.Property(c => c.EmailID).HasMaxLength(100).IsRequired();
        builder.Property(c => c.DeviceID).HasMaxLength(200).IsRequired();
        builder.Property(c => c.OTP).HasMaxLength(10).IsUnicode(false);
        builder.Property(c => c.CreatedOn).IsRequired();

        builder.HasMany(c => c.Addresses)
            .WithOne()
            .HasForeignKey(a => a.AddressLinkID)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
