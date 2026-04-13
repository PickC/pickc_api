using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class DriverAttachmentConfiguration : IEntityTypeConfiguration<DriverAttachment>
{
    public void Configure(EntityTypeBuilder<DriverAttachment> builder)
    {
        builder.ToTable("DriverAttachments", "Master");

        builder.HasKey(a => a.AttachmentId);
        builder.Property(a => a.AttachmentId).HasMaxLength(100).IsUnicode(false);
        builder.Property(a => a.DriverID).HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(a => a.LookupCode).HasMaxLength(50).IsUnicode(false);
        builder.Property(a => a.ImagePath).HasMaxLength(100).IsUnicode(false);
    }
}
