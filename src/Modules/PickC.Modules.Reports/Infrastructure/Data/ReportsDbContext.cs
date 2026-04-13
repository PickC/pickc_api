using Microsoft.EntityFrameworkCore;

namespace PickC.Modules.Reports.Infrastructure.Data;

public class ReportsDbContext : DbContext
{
    public ReportsDbContext(DbContextOptions<ReportsDbContext> options) : base(options) { }

    public DbSet<ReportInvoice> Invoices => Set<ReportInvoice>();
    public DbSet<ReportTrip> Trips => Set<ReportTrip>();
    public DbSet<ReportBooking> Bookings => Set<ReportBooking>();
    public DbSet<ReportDriver> Drivers => Set<ReportDriver>();
    public DbSet<ReportCustomer> Customers => Set<ReportCustomer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Invoice (Operation schema, composite key)
        modelBuilder.Entity<ReportInvoice>(e => {
            e.ToTable("Invoice", "Operation");
            e.HasKey(x => new { x.InvoiceNo, x.TripID });
        });

        // Trip (Operation schema, TripID is PK)
        modelBuilder.Entity<ReportTrip>(e => {
            e.ToTable("Trip", "Operation");
            e.HasKey(x => x.TripID);
        });

        // Booking (Operation schema, BookingNo is PK)
        modelBuilder.Entity<ReportBooking>(e => {
            e.ToTable("Booking", "Operation");
            e.HasKey(x => x.BookingNo);
        });

        // Driver (Master schema, DriverID is PK)
        modelBuilder.Entity<ReportDriver>(e => {
            e.ToTable("Driver", "Master");
            e.HasKey(x => x.DriverID);
        });

        // Customer (Master schema, MobileNo is PK)
        modelBuilder.Entity<ReportCustomer>(e => {
            e.ToTable("Customer", "Master");
            e.HasKey(x => x.MobileNo);
        });

        base.OnModelCreating(modelBuilder);
    }
}

public class ReportInvoice
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string TripID { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public decimal TripAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TipAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public short PaymentType { get; set; }
    public decimal PaidAmount { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool IsMailSent { get; set; }
    public string BookingNo { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}

public class ReportTrip
{
    public string TripID { get; set; } = string.Empty;
    public DateTime TripDate { get; set; }
    public string CustomerMobile { get; set; } = string.Empty;
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public string? LocationFrom { get; set; }
    public string? LocationTo { get; set; }
    public decimal Distance { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public decimal TripMinutes { get; set; }
    public string? BookingNo { get; set; }
}

public class ReportBooking
{
    public string BookingNo { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public string CustomerID { get; set; } = string.Empty;
    public string LocationFrom { get; set; } = string.Empty;
    public string LocationTo { get; set; } = string.Empty;
    public short VehicleType { get; set; }
    public string DriverID { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public bool IsConfirm { get; set; }
    public bool IsCancel { get; set; }
    public bool IsComplete { get; set; }
    public string CargoType { get; set; } = string.Empty;
    public int Status { get; set; }
}

public class ReportDriver
{
    public string DriverID { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public string MobileNo { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
}

public class ReportCustomer
{
    public string MobileNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EmailID { get; set; } = string.Empty;
}
