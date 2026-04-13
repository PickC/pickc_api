using Microsoft.EntityFrameworkCore;
using PickC.Modules.Reports.Application.DTOs;
using PickC.Modules.Reports.Infrastructure.Data;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Reports.Application.Services;

public class ReportQueryService : IReportQueryService
{
    private readonly ReportsDbContext _db;

    public ReportQueryService(ReportsDbContext db)
    {
        _db = db;
    }

    public async Task<DailyBookingReportSummary> GetDailyBookingsAsync(DateTime date, CancellationToken ct = default)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var bookings = await _db.Bookings.AsNoTracking()
            .Where(b => b.BookingDate >= dayStart && b.BookingDate < dayEnd)
            .Select(b => new DailyBookingReportDto
            {
                BookingNo = b.BookingNo,
                BookingDate = b.BookingDate,
                CustomerID = b.CustomerID,
                DriverID = b.DriverID,
                VehicleType = b.VehicleType,
                LocationFrom = b.LocationFrom,
                LocationTo = b.LocationTo,
                CargoType = b.CargoType,
                Status = b.Status,
                IsCancel = b.IsCancel,
                IsComplete = b.IsComplete
            })
            .ToListAsync(ct);

        return new DailyBookingReportSummary
        {
            Bookings = bookings,
            Total = bookings.Count,
            Confirmed = bookings.Count(b => b.IsComplete == false && b.IsCancel == false),
            Completed = bookings.Count(b => b.IsComplete),
            Cancelled = bookings.Count(b => b.IsCancel)
        };
    }

    public async Task<List<DailyTripReportDto>> GetDailyTripsAsync(DateTime date, CancellationToken ct = default)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        return await _db.Trips.AsNoTracking()
            .Where(t => t.TripDate >= dayStart && t.TripDate < dayEnd)
            .Select(t => new DailyTripReportDto
            {
                TripID = t.TripID,
                TripDate = t.TripDate,
                CustomerMobile = t.CustomerMobile,
                DriverID = t.DriverID,
                VehicleNo = t.VehicleNo,
                LocationFrom = t.LocationFrom,
                LocationTo = t.LocationTo,
                DistanceKm = t.Distance,
                TripMinutes = t.TripMinutes,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                BookingNo = t.BookingNo
            })
            .ToListAsync(ct);
    }

    public async Task<DailyPaymentReportSummary> GetDailyPaymentsAsync(DateTime date, CancellationToken ct = default)
    {
        var dayStart = date.Date;
        var dayEnd = dayStart.AddDays(1);

        var payments = await (
            from inv in _db.Invoices.AsNoTracking()
            join trip in _db.Trips.AsNoTracking() on inv.TripID equals trip.TripID
            where inv.InvoiceDate >= dayStart && inv.InvoiceDate < dayEnd
            select new DailyPaymentReportDto
            {
                InvoiceNo = inv.InvoiceNo,
                BookingNo = inv.BookingNo,
                TripID = inv.TripID,
                InvoiceDate = inv.InvoiceDate,
                CustomerMobile = trip.CustomerMobile,
                DriverID = trip.DriverID,
                TripAmount = inv.TripAmount,
                TaxAmount = inv.TaxAmount,
                TipAmount = inv.TipAmount,
                TotalAmount = inv.TotalAmount,
                PaymentType = inv.PaymentType,
                PaidAmount = inv.PaidAmount,
                IsPaid = inv.IsPaid,
                PaidDate = inv.PaidDate
            }
        ).ToListAsync(ct);

        return new DailyPaymentReportSummary
        {
            Payments = payments,
            TotalTripAmount = payments.Sum(p => p.TripAmount),
            TotalTaxAmount = payments.Sum(p => p.TaxAmount),
            TotalTipAmount = payments.Sum(p => p.TipAmount),
            TotalAmount = payments.Sum(p => p.TotalAmount),
            TotalPaidAmount = payments.Sum(p => p.PaidAmount)
        };
    }

    public async Task<SummaryReportDto> GetSummaryAsync(DateTime startDate, DateTime endDate, CancellationToken ct = default)
    {
        var rangeStart = startDate.Date;
        var rangeEnd = endDate.Date.AddDays(1);

        // Booking counts
        var bookingStats = await _db.Bookings.AsNoTracking()
            .Where(b => b.BookingDate >= rangeStart && b.BookingDate < rangeEnd)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Confirmed = g.Count(b => b.IsConfirm),
                Completed = g.Count(b => b.IsComplete),
                Cancelled = g.Count(b => b.IsCancel)
            })
            .FirstOrDefaultAsync(ct);

        // Trip stats
        var tripStats = await _db.Trips.AsNoTracking()
            .Where(t => t.TripDate >= rangeStart && t.TripDate < rangeEnd)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                TotalDistance = g.Sum(t => t.Distance)
            })
            .FirstOrDefaultAsync(ct);

        // Active driver count
        var activeDrivers = await _db.Trips.AsNoTracking()
            .Where(t => t.TripDate >= rangeStart && t.TripDate < rangeEnd)
            .Select(t => t.DriverID)
            .Distinct()
            .CountAsync(ct);

        // Invoice financial stats
        var invoiceStats = await _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceDate >= rangeStart && i.InvoiceDate < rangeEnd)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalRevenue = g.Sum(i => i.TotalAmount),
                TotalTax = g.Sum(i => i.TaxAmount),
                TotalTips = g.Sum(i => i.TipAmount),
                TotalPaid = g.Sum(i => i.PaidAmount),
                CashCount = g.Count(i => i.PaymentType == 1),
                CashAmount = g.Sum(i => i.PaymentType == 1 ? i.TotalAmount : 0),
                CreditCount = g.Count(i => i.PaymentType == 2),
                CreditAmount = g.Sum(i => i.PaymentType == 2 ? i.TotalAmount : 0),
                OnlineCount = g.Count(i => i.PaymentType == 3),
                OnlineAmount = g.Sum(i => i.PaymentType == 3 ? i.TotalAmount : 0),
            })
            .FirstOrDefaultAsync(ct);

        // Top 5 drivers by earnings (join trips + invoices, group by driver)
        var topDriverData = await (
            from trip in _db.Trips.AsNoTracking()
                .Where(t => t.TripDate >= rangeStart && t.TripDate < rangeEnd)
            join inv in _db.Invoices.AsNoTracking()
                .Where(i => i.InvoiceDate >= rangeStart && i.InvoiceDate < rangeEnd)
                on trip.TripID equals inv.TripID
            group new { trip, inv } by trip.DriverID into g
            orderby g.Sum(x => x.inv.TotalAmount) descending
            select new
            {
                DriverID = g.Key,
                TotalEarnings = g.Sum(x => x.inv.TotalAmount),
                TotalTrips = g.Count()
            }
        ).Take(5).ToListAsync(ct);

        var driverIds = topDriverData.Select(d => d.DriverID).ToList();
        var drivers = await _db.Drivers.AsNoTracking()
            .Where(d => driverIds.Contains(d.DriverID))
            .ToListAsync(ct);

        var topDrivers = topDriverData
            .Select(d => new TopDriverDto
            {
                DriverID = d.DriverID,
                DriverName = drivers.FirstOrDefault(dr => dr.DriverID == d.DriverID)?.DriverName ?? d.DriverID,
                TotalTrips = d.TotalTrips,
                TotalEarnings = d.TotalEarnings
            })
            .ToList();

        return new SummaryReportDto
        {
            TotalBookings = bookingStats?.Total ?? 0,
            ConfirmedBookings = bookingStats?.Confirmed ?? 0,
            CompletedBookings = bookingStats?.Completed ?? 0,
            CancelledBookings = bookingStats?.Cancelled ?? 0,
            TotalTrips = tripStats?.Total ?? 0,
            TotalDistanceKm = tripStats?.TotalDistance ?? 0,
            TotalRevenue = invoiceStats?.TotalRevenue ?? 0,
            TotalTaxAmount = invoiceStats?.TotalTax ?? 0,
            TotalTipAmount = invoiceStats?.TotalTips ?? 0,
            TotalPaidAmount = invoiceStats?.TotalPaid ?? 0,
            OutstandingAmount = (invoiceStats?.TotalRevenue ?? 0) - (invoiceStats?.TotalPaid ?? 0),
            CashPaymentCount = invoiceStats?.CashCount ?? 0,
            CashPaymentAmount = invoiceStats?.CashAmount ?? 0,
            CreditPaymentCount = invoiceStats?.CreditCount ?? 0,
            CreditPaymentAmount = invoiceStats?.CreditAmount ?? 0,
            OnlinePaymentCount = invoiceStats?.OnlineCount ?? 0,
            OnlinePaymentAmount = invoiceStats?.OnlineAmount ?? 0,
            ActiveDrivers = activeDrivers,
            TopDrivers = topDrivers
        };
    }

    public async Task<List<DriverEarningsReportDto>> GetDriverEarningsAsync(DateTime startDate, DateTime endDate, string? driverId, CancellationToken ct = default)
    {
        var rangeStart = startDate.Date;
        var rangeEnd = endDate.Date.AddDays(1);

        var invoicesQuery = _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceDate >= rangeStart && i.InvoiceDate < rangeEnd);

        var tripsQuery = _db.Trips.AsNoTracking()
            .Where(t => t.TripDate >= rangeStart && t.TripDate < rangeEnd);

        if (!string.IsNullOrEmpty(driverId))
            tripsQuery = tripsQuery.Where(t => t.DriverID == driverId);

        var joined = await (
            from trip in tripsQuery
            join inv in invoicesQuery on trip.TripID equals inv.TripID
            join driver in _db.Drivers.AsNoTracking() on trip.DriverID equals driver.DriverID
            select new
            {
                trip.DriverID,
                driver.DriverName,
                driver.MobileNo,
                trip.Distance,
                inv.TripAmount,
                inv.TaxAmount,
                inv.TipAmount,
                inv.TotalAmount,
                inv.PaidAmount
            }
        ).ToListAsync(ct);

        return joined
            .GroupBy(x => new { x.DriverID, x.DriverName, x.MobileNo })
            .Select(g => new DriverEarningsReportDto
            {
                DriverID = g.Key.DriverID,
                DriverName = g.Key.DriverName,
                MobileNo = g.Key.MobileNo,
                TotalTrips = g.Count(),
                TotalDistanceKm = g.Sum(x => x.Distance),
                TripAmount = g.Sum(x => x.TripAmount),
                TaxAmount = g.Sum(x => x.TaxAmount),
                TipAmount = g.Sum(x => x.TipAmount),
                TotalEarnings = g.Sum(x => x.TotalAmount),
                PaidAmount = g.Sum(x => x.PaidAmount),
                Outstanding = g.Sum(x => x.TotalAmount - x.PaidAmount)
            })
            .ToList();
    }

    public async Task<DriverTipReportSummary> GetDriverTipsAsync(DateTime startDate, DateTime endDate, string? driverId, CancellationToken ct = default)
    {
        var rangeStart = startDate.Date;
        var rangeEnd = endDate.Date.AddDays(1);

        var invoicesQuery = _db.Invoices.AsNoTracking()
            .Where(i => i.InvoiceDate >= rangeStart && i.InvoiceDate < rangeEnd && i.TipAmount > 0);

        var tripsQuery = _db.Trips.AsNoTracking()
            .Where(t => t.TripDate >= rangeStart && t.TripDate < rangeEnd);

        if (!string.IsNullOrEmpty(driverId))
            tripsQuery = tripsQuery.Where(t => t.DriverID == driverId);

        var tips = await (
            from inv in invoicesQuery
            join trip in tripsQuery on inv.TripID equals trip.TripID
            join driver in _db.Drivers.AsNoTracking() on trip.DriverID equals driver.DriverID
            select new DriverTipReportDto
            {
                Date = inv.InvoiceDate,
                InvoiceNo = inv.InvoiceNo,
                TripID = inv.TripID,
                BookingNo = inv.BookingNo,
                CustomerMobile = trip.CustomerMobile,
                DriverID = trip.DriverID,
                DriverName = driver.DriverName,
                TipAmount = inv.TipAmount
            }
        ).ToListAsync(ct);

        return new DriverTipReportSummary
        {
            Tips = tips,
            TotalTips = tips.Sum(t => t.TipAmount)
        };
    }

    public async Task<InvoiceReportDto> GetInvoiceAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        var result = await (
            from inv in _db.Invoices.AsNoTracking()
            join trip in _db.Trips.AsNoTracking() on inv.TripID equals trip.TripID
            join driver in _db.Drivers.AsNoTracking() on trip.DriverID equals driver.DriverID
            join customerRow in _db.Customers.AsNoTracking() on trip.CustomerMobile equals customerRow.MobileNo into customerGroup
            from customer in customerGroup.DefaultIfEmpty()
            where inv.InvoiceNo == invoiceNo && inv.TripID == tripId
            select new InvoiceReportDto
            {
                InvoiceNo = inv.InvoiceNo,
                TripID = inv.TripID,
                InvoiceDate = inv.InvoiceDate,
                BookingNo = inv.BookingNo,
                CustomerMobile = trip.CustomerMobile,
                CustomerName = customer != null ? customer.Name : string.Empty,
                CustomerEmail = customer != null ? customer.EmailID : string.Empty,
                DriverID = trip.DriverID,
                DriverName = driver.DriverName,
                VehicleNo = trip.VehicleNo,
                LocationFrom = trip.LocationFrom,
                LocationTo = trip.LocationTo,
                DistanceKm = trip.Distance,
                TripMinutes = trip.TripMinutes,
                StartTime = trip.StartTime,
                EndTime = trip.EndTime,
                TripAmount = inv.TripAmount,
                TaxAmount = inv.TaxAmount,
                TipAmount = inv.TipAmount,
                TotalAmount = inv.TotalAmount,
                PaymentType = inv.PaymentType,
                PaidAmount = inv.PaidAmount,
                IsPaid = inv.IsPaid,
                PaidDate = inv.PaidDate
            }
        ).FirstOrDefaultAsync(ct);

        if (result is null)
            throw new NotFoundException("Invoice", $"{invoiceNo}/{tripId}");

        return result;
    }

    // NOTE: This is the only write operation in this read-oriented context.
    // It exists here to avoid a cross-module dependency. A future refactor
    // should route this through an event or the Billing module's own service.
    public async Task MarkInvoiceMailSentAsync(string invoiceNo, string tripId, CancellationToken ct = default)
    {
        var updated = await _db.Invoices
            .Where(i => i.InvoiceNo == invoiceNo && i.TripID == tripId)
            .ExecuteUpdateAsync(s => s.SetProperty(i => i.IsMailSent, true), ct);

        if (updated == 0)
            throw new NotFoundException("Invoice", $"{invoiceNo}/{tripId}");
    }
}
