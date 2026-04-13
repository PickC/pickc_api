using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Infrastructure.Data;

public class MasterDbContext : DbContext
{
    public MasterDbContext(DbContextOptions<MasterDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<LookUp> LookUps => Set<LookUp>();
    public DbSet<BankDetails> BankDetails => Set<BankDetails>();
    public DbSet<DriverAttachment> DriverAttachments => Set<DriverAttachment>();
    public DbSet<DriverBankDetails> DriverBankDetails => Set<DriverBankDetails>();
    public DbSet<DriverRating> DriverRatings => Set<DriverRating>();
    public DbSet<DriverReferral> DriverReferrals => Set<DriverReferral>();
    public DbSet<Operator> Operators => Set<Operator>();
    public DbSet<RateCard> RateCards => Set<RateCard>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<VehicleConfig> VehicleConfigs => Set<VehicleConfig>();
    public DbSet<VehicleManufacturer> VehicleManufacturers => Set<VehicleManufacturer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MasterDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
