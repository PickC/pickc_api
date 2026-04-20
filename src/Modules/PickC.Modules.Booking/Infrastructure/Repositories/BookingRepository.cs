using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PickC.Modules.Booking.Domain.Entities;
using PickC.Modules.Booking.Domain.Interfaces;
using PickC.Modules.Booking.Infrastructure.Data;
using PickC.SharedKernel.Helpers;
using System.Data;

namespace PickC.Modules.Booking.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BookingDbContext _context;

    public BookingRepository(BookingDbContext context)
    {
        _context = context;
    }

    public async Task<List<Domain.Entities.Booking>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);
    }

    public async Task<Domain.Entities.Booking?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingNo == bookingNo, ct);
    }

    public async Task<List<Domain.Entities.Booking>> GetByCustomerAsync(string customerId, CancellationToken ct = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.CustomerID == customerId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);
    }

    public async Task<List<Domain.Entities.Booking>> GetByDriverAsync(string driverId, CancellationToken ct = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.DriverID == driverId && b.IsConfirm)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);
    }

    public async Task<string> SaveAsync(Domain.Entities.Booking booking, CancellationToken ct = default)
    {
        var isNew = string.IsNullOrEmpty(booking.BookingNo) ||
                    !await _context.Bookings.AnyAsync(b => b.BookingNo == booking.BookingNo, ct);

        if (isNew)
        {
            // Cancel all unconfirmed, non-cancelled bookings for this customer (mirrors SP logic)
            await _context.Bookings
                .Where(b => b.CustomerID == booking.CustomerID && !b.IsConfirm && !b.IsCancel)
                .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsCancel, true), ct);

            // Generate BookingNo via usp_GenerateDocumentNumber
            var bookingNoParam = new SqlParameter
            {
                ParameterName = "@BookingNo",
                SqlDbType = SqlDbType.NVarChar,
                Size = 50,
                Value = string.Empty,
                Direction = ParameterDirection.InputOutput
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC [Utility].[usp_GenerateDocumentNumber] @DocumentType, @Dt, @UserId, @BookingNo OUTPUT",
                new SqlParameter("@DocumentType", "Booking"),
                new SqlParameter("@Dt", DateTime.UtcNow),
                new SqlParameter("@UserId", "ADMIN"),
                bookingNoParam);

            booking.BookingNo = bookingNoParam.Value?.ToString() ?? string.Empty;
            booking.BookingDate = IstClock.Now;
            _context.Bookings.Add(booking);
        }
        else
        {
            var existing = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNo == booking.BookingNo, ct);
            _context.Entry(existing!).CurrentValues.SetValues(booking);
        }

        await _context.SaveChangesAsync(ct);
        return booking.BookingNo;
    }

    public async Task<bool> DeleteAsync(string bookingNo, CancellationToken ct = default)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingNo == bookingNo, ct);

        if (booking is null) return false;

        _context.Bookings.Remove(booking);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> ConfirmAsync(string bookingNo, string driverId, string vehicleNo, CancellationToken ct = default)
    {
        return await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && !b.IsConfirm && !b.IsCancel)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsConfirm, true)
                .SetProperty(b => b.ConfirmDate, IstClock.Now)
                .SetProperty(b => b.DriverID, driverId)
                .SetProperty(b => b.VehicleNo, vehicleNo), ct) > 0;
    }

    public async Task<bool> CancelByCustomerAsync(string bookingNo, string cancelRemarks, CancellationToken ct = default)
    {
        return await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && !b.IsCancel && !b.IsComplete)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsCancel, true)
                .SetProperty(b => b.CancelTime, IstClock.Now)
                .SetProperty(b => b.CancelRemarks, cancelRemarks), ct) > 0;
    }

    public async Task<bool> CancelByDriverAsync(string bookingNo, string driverId, string cancelRemarks, CancellationToken ct = default)
    {
        var updated = await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && !b.IsCancel && !b.IsComplete)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsCancelByDriver, true)
                .SetProperty(b => b.DriverCancelDateTime, IstClock.Now)
                .SetProperty(b => b.DriverCancelRemarks, cancelRemarks)
                .SetProperty(b => b.IsConfirm, false)
                .SetProperty(b => b.DriverID, string.Empty)
                .SetProperty(b => b.VehicleNo, string.Empty), ct) > 0;

        if (updated)
        {
            _context.DriverCancellationHistories.Add(new DriverCancellationHistory
            {
                DriverID = driverId,
                BookingNo = bookingNo,
                CancelRemarks = cancelRemarks,
                CancelTime = IstClock.Now
            });
            await _context.SaveChangesAsync(ct);
        }

        return updated;
    }

    public async Task<bool> CompleteAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && b.IsConfirm && !b.IsCancel && !b.IsComplete)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsComplete, true)
                .SetProperty(b => b.CompleteTime, IstClock.Now), ct) > 0;
    }

    public async Task<bool> UpdateReachPickUpAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && b.IsConfirm)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsReachPickUp, true)
                .SetProperty(b => b.PickupReachDateTime, IstClock.Now), ct) > 0;
    }

    public async Task<bool> UpdateReachDestinationAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Bookings
            .Where(b => b.BookingNo == bookingNo && b.IsConfirm)
            .ExecuteUpdateAsync(s => s
                .SetProperty(b => b.IsReachDestination, true)
                .SetProperty(b => b.DestinationReachDateTime, IstClock.Now), ct) > 0;
    }

    public async Task<List<Domain.Entities.Booking>> SearchByDateAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.BookingDate >= fromDate && b.BookingDate <= toDate)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);
    }

    public async Task<List<Domain.Entities.Booking>> SearchByStatusAsync(int status, CancellationToken ct = default)
    {
        return await _context.Bookings
            .AsNoTracking()
            .Where(b => b.Status == status)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync(ct);
    }

    public async Task<List<Domain.Entities.Booking>> GetNearBookingsForDriverAsync(
        decimal latitude, decimal longitude, double rangeKm, CancellationToken ct = default)
    {
        // Get unconfirmed, non-cancelled bookings then filter by GPS distance in memory
        var pendingBookings = await _context.Bookings
            .AsNoTracking()
            .Where(b => !b.IsConfirm && !b.IsCancel && !b.IsComplete)
            .ToListAsync(ct);

        return pendingBookings
            .Where(b => CoordinateDistanceHelper.CalculateDistanceKm(
                (double)latitude, (double)longitude,
                (double)b.Latitude, (double)b.Longitude) <= rangeKm)
            .OrderBy(b => CoordinateDistanceHelper.CalculateDistanceKm(
                (double)latitude, (double)longitude,
                (double)b.Latitude, (double)b.Longitude))
            .ToList();
    }
}
