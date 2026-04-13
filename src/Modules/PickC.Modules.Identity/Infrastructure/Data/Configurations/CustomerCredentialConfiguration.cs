using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Identity.Domain.Entities;

namespace PickC.Modules.Identity.Infrastructure.Data.Configurations;

public class CustomerCredentialConfiguration : IEntityTypeConfiguration<CustomerCredential>
{
    public void Configure(EntityTypeBuilder<CustomerCredential> builder)
    {
        builder.ToTable("Customer", "Master");

        builder.HasKey(c => c.MobileNo);
        builder.Property(c => c.MobileNo).HasMaxLength(20);
        builder.Property(c => c.Password).HasMaxLength(15).IsRequired();
    }
}
