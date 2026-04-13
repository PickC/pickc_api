using Microsoft.EntityFrameworkCore;
using PickC.Modules.Identity.Domain.Entities;
using PickC.SharedKernel.Domain;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    public DbSet<CustomerCredential> CustomerCredentials => Set<CustomerCredential>();
    public DbSet<CustomerLogin> CustomerLogins => Set<CustomerLogin>();
    public DbSet<DriverCredential> DriverCredentials => Set<DriverCredential>();
    public DbSet<DriverActivity> DriverActivities => Set<DriverActivity>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedDate = IstClock.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedDate = IstClock.Now;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
