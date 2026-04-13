using Microsoft.EntityFrameworkCore;
using PickC.Modules.Billing.Application.DTOs;
using PickC.Modules.Billing.Infrastructure.Data;

namespace PickC.Modules.Billing.Application.Services;

public interface IDriverBillingService
{
    Task<List<DriverPaymentDto>> GetDriverPaymentsAsync(string driverId, CancellationToken ct = default);
    Task<DriverSummaryDto> GetDriverSummaryAsync(string driverId, CancellationToken ct = default);
    Task<List<DriverTripInvoiceDto>> GetDriverInvoicesAsync(string driverId, CancellationToken ct = default);
}

public class DriverBillingService : IDriverBillingService
{
    private readonly BillingDbContext _context;

    public DriverBillingService(BillingDbContext context)
    {
        _context = context;
    }

    // Replaces Operation.usp_GetDriverPayments
    // Joins Invoice + Trip, groups by date, sums TripAmount
    public async Task<List<DriverPaymentDto>> GetDriverPaymentsAsync(string driverId, CancellationToken ct = default)
    {
        var payments = await _context.Database
            .SqlQuery<DriverPaymentDto>($"""
                SELECT Convert(varchar(10), A.InvoiceDate, 101) AS CreatedOn,
                       Sum(A.TripAmount) AS PaidAmount
                FROM [Operation].[Invoice] AS A
                INNER JOIN [Operation].[Trip] AS B ON A.TripID = B.TripID
                WHERE B.DriverID = {driverId}
                GROUP BY Convert(varchar(10), A.InvoiceDate, 101)
                """)
            .ToListAsync(ct);

        return payments;
    }

    // Returns per-trip invoice list for the driver accounts page (supports payment-type filtering client-side)
    public async Task<List<DriverTripInvoiceDto>> GetDriverInvoicesAsync(string driverId, CancellationToken ct = default)
    {
        var invoices = await _context.Database
            .SqlQuery<DriverTripInvoiceDto>($"""
                SELECT A.InvoiceNo,
                       A.TripID,
                       A.BookingNo,
                       B.TripDate,
                       ISNULL(B.LocationFrom, '') AS LocationFrom,
                       ISNULL(B.LocationTo,   '') AS LocationTo,
                       A.TotalAmount,
                       A.PaymentType,
                       A.IsPaid
                FROM [Operation].[Invoice] AS A
                INNER JOIN [Operation].[Trip] AS B ON A.TripID = B.TripID
                WHERE B.DriverID = {driverId}
                ORDER BY B.TripDate DESC
                """)
            .ToListAsync(ct);

        return invoices;
    }

    // Replaces Operation.usp_GetDriverSummary
    // Complex aggregation across Invoice, Trip, Booking
    public async Task<DriverSummaryDto> GetDriverSummaryAsync(string driverId, CancellationToken ct = default)
    {
        var summary = await _context.Database
            .SqlQuery<DriverSummaryDto>($"""
                SELECT
                    CAST(0.00 AS DECIMAL(18,2)) AS CurrentBalance,
                    ISNULL((SELECT SUM(A.TotalAmount) FROM [Operation].[Invoice] AS A
                        INNER JOIN [Operation].[Trip] AS B ON A.TripID = B.TripID
                        WHERE B.DriverID = {driverId} AND CAST(A.CreatedOn AS DATE) = CAST(GETUTCDATE() AS DATE)), 0.00) AS TodaySummary,
                    ISNULL((SELECT COUNT(*) FROM [Operation].[Booking]
                        WHERE DriverID = {driverId} AND CAST(BookingDate AS DATE) = CAST(GETUTCDATE() AS DATE)), 0) AS TodayBookings,
                    ISNULL((SELECT SUM(C.PaidAmount) FROM [Operation].[Invoice] AS C
                        INNER JOIN [Operation].[Trip] AS D ON C.TripID = D.TripID
                        WHERE D.DriverID = {driverId} AND CAST(C.CreatedOn AS DATE) = CAST(GETUTCDATE() AS DATE)), 0.00) AS TodayPayment,
                    ISNULL((SELECT TOP 1 E.PaidAmount FROM [Operation].[Invoice] AS E
                        INNER JOIN [Operation].[Trip] AS F ON E.TripID = F.TripID
                        WHERE F.DriverID = {driverId} AND CAST(E.CreatedOn AS DATE) = CAST(GETUTCDATE() AS DATE)
                        ORDER BY E.CreatedOn DESC), 0.00) AS LastPayment
                """)
            .FirstOrDefaultAsync(ct);

        return summary ?? new DriverSummaryDto();
    }
}
