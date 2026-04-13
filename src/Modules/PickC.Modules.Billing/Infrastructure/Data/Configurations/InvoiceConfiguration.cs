using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Billing.Domain.Entities;

namespace PickC.Modules.Billing.Infrastructure.Data.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoice", "Operation");

        builder.HasKey(i => new { i.InvoiceNo, i.TripID });
        builder.Property(i => i.InvoiceNo).HasMaxLength(50);
        builder.Property(i => i.TripID).HasMaxLength(50);
        builder.Property(i => i.InvoiceDate).HasDefaultValueSql("getutcdate()");
        builder.Property(i => i.TripAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.TaxAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.TipAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.TotalAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.PaymentType).HasDefaultValue((short)0);
        builder.Property(i => i.PaidAmount).HasColumnType("numeric(18,2)").HasDefaultValue(0m);
        builder.Property(i => i.CreatedOn).HasDefaultValueSql("getutcdate()");
        builder.Property(i => i.IsMailSent).HasDefaultValue(false);
        builder.Property(i => i.BookingNo).HasMaxLength(50).IsUnicode(false).HasDefaultValue("");
        builder.Property(i => i.IsPaid).IsRequired();
    }
}
