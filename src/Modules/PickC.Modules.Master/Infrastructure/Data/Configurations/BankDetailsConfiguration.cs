using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class BankDetailsConfiguration : IEntityTypeConfiguration<BankDetails>
{
    public void Configure(EntityTypeBuilder<BankDetails> builder)
    {
        builder.ToTable("BankDetails", "Master");

        builder.HasKey(b => b.OperatorBankID);
        builder.Property(b => b.OperatorBankID).HasMaxLength(20);
        builder.Property(b => b.BankName).HasMaxLength(100);
        builder.Property(b => b.Branch).HasMaxLength(100);
        builder.Property(b => b.AccountNumber).HasMaxLength(50);
        builder.Property(b => b.AccountType).HasMaxLength(50);
        builder.Property(b => b.IFSC).HasMaxLength(20);
    }
}
