using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Identity.Domain.Entities;

namespace PickC.Modules.Identity.Infrastructure.Data.Configurations;

public class CustomerLoginConfiguration : IEntityTypeConfiguration<CustomerLogin>
{
    public void Configure(EntityTypeBuilder<CustomerLogin> builder)
    {
        builder.ToTable("CustomerLogin", "Operation");

        builder.HasKey(c => c.TokenNo);
        builder.Property(c => c.MobileNo).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Status).IsRequired();
        builder.Property(c => c.CurrentLat).HasColumnType("decimal(18,10)");
        builder.Property(c => c.CurrentLong).HasColumnType("decimal(18,10)");

        builder.HasIndex(c => c.MobileNo);
    }
}
