using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data.Configurations;

public class LookUpConfiguration : IEntityTypeConfiguration<LookUp>
{
    public void Configure(EntityTypeBuilder<LookUp> builder)
    {
        builder.ToTable("LookUp", "Config");

        builder.HasKey(l => l.LookupID);
        builder.Property(l => l.LookupID).ValueGeneratedOnAdd();
        builder.Property(l => l.LookupCode).HasMaxLength(20).IsRequired();
        builder.Property(l => l.LookupDescription).HasMaxLength(50).IsRequired();
        builder.Property(l => l.LookupCategory).HasMaxLength(20).IsRequired();
        builder.Property(l => l.Image).HasColumnType("nvarchar(max)");

        builder.HasIndex(l => l.LookupCategory);
    }
}
