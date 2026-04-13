using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PickC.Modules.Identity.Domain.Entities;

namespace PickC.Modules.Identity.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshToken", "Security");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("TokenId").HasMaxLength(50);
        builder.Property(r => r.UserId).HasMaxLength(50).IsRequired();
        builder.Property(r => r.UserType).HasMaxLength(20).IsRequired();
        builder.Property(r => r.Token).HasMaxLength(500).IsRequired();

        builder.HasIndex(r => new { r.UserId, r.Token });
    }
}
