using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class DriverBankDetailsConfiguration : IEntityTypeConfiguration<DriverBankDetails>
{
    public void Configure(EntityTypeBuilder<DriverBankDetails> builder)
    {
        builder.ToTable("DriverBankDetails", "Master");

        builder.HasKey(b => b.DriverID);
        builder.Property(b => b.DriverID).HasMaxLength(50).IsUnicode(false);
        builder.Property(b => b.BankName).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(b => b.Branch).HasMaxLength(50).IsUnicode(false);
        builder.Property(b => b.AccountNumber).HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(b => b.AccountType).HasMaxLength(50).IsUnicode(false);
        builder.Property(b => b.IFSC).HasMaxLength(15).IsUnicode(false).IsRequired();
        builder.Property(b => b.AccountName).HasMaxLength(100);
    }
}
