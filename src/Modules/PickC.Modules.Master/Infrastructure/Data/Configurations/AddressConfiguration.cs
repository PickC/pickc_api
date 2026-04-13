using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Address", "Master");

        builder.HasKey(a => a.AddressId);
        builder.Property(a => a.AddressId).ValueGeneratedOnAdd();
        builder.Property(a => a.AddressLinkID).HasMaxLength(50).IsRequired();
        builder.Property(a => a.AddressType).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(a => a.Address1).HasMaxLength(100).IsRequired();
        builder.Property(a => a.Address2).HasMaxLength(100);
        builder.Property(a => a.Address3).HasMaxLength(50).IsUnicode(false);
        builder.Property(a => a.Address4).HasMaxLength(50).IsUnicode(false);
        builder.Property(a => a.CityName).HasMaxLength(40);
        builder.Property(a => a.StateName).HasMaxLength(40);
        builder.Property(a => a.CountryCode).HasMaxLength(2).IsUnicode(false);
        builder.Property(a => a.ZipCode).HasMaxLength(6).IsUnicode(false);
        builder.Property(a => a.TelNo).HasMaxLength(20).IsUnicode(false);
        builder.Property(a => a.FaxNo).HasMaxLength(20).IsUnicode(false);
        builder.Property(a => a.MobileNo).HasMaxLength(20).IsUnicode(false);
        builder.Property(a => a.Contact).HasMaxLength(20);
        builder.Property(a => a.Email).HasMaxLength(50).IsUnicode(false);
        builder.Property(a => a.WebSite).HasMaxLength(50).IsUnicode(false);
        builder.Property(a => a.CreatedBy).HasMaxLength(25).IsUnicode(false).IsRequired();
        builder.Property(a => a.ModifiedBy).HasMaxLength(25).IsUnicode(false);

        builder.Ignore(a => a.FullAddress);
    }
}
