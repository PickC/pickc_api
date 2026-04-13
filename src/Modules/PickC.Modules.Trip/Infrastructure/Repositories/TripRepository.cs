using Microsoft.EntityFrameworkCore;
using PickC.Modules.Trip.Domain.Interfaces;
using PickC.Modules.Trip.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Trip.Infrastructure.Repositories;

public class TripRepository : ITripRepository
{
    private readonly TripDbContext _context;

    public TripRepository(TripDbContext context)
    {
        _context = context;
    }

    public async Task<List<Domain.Entities.Trip>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Trips
            .AsNoTracking()
            .OrderByDescending(t => t.TripDate)
            .ToListAsync(ct);
    }

    public async Task<Domain.Entities.Trip?> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        return await _context.Trips
            .FirstOrDefaultAsync(t => t.TripID == tripId, ct);
    }

    public async Task<Domain.Entities.Trip?> GetCurrentByDriverAsync(string driverId, CancellationToken ct = default)
    {
        return await _context.Trips
            .AsNoTracking()
            .Where(t => t.DriverID == driverId && t.EndTime == null)
            .OrderByDescending(t => t.StartTime)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Domain.Entities.Trip?> GetCurrentByCustomerAsync(string customerMobile, CancellationToken ct = default)
    {
        return await _context.Trips
            .AsNoTracking()
            .Where(t => t.CustomerMobile == customerMobile && t.EndTime == null)
            .OrderByDescending(t => t.StartTime)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<Domain.Entities.Trip>> GetByDriverAsync(string driverId, CancellationToken ct = default)
    {
        return await _context.Trips
            .AsNoTracking()
            .Where(t => t.DriverID == driverId)
            .OrderByDescending(t => t.TripDate)
            .ToListAsync(ct);
    }

    public async Task<List<Domain.Entities.Trip>> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default)
    {
        return await _context.Trips
            .AsNoTracking()
            .Where(t => t.BookingNo == bookingNo)
            .ToListAsync(ct);
    }

    public async Task<string?> SaveAsync(Domain.Entities.Trip trip, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(trip.TripID))
            trip.TripID = await GenerateTripIdAsync(ct);

        var existing = await _context.Trips
            .FirstOrDefaultAsync(t => t.TripID == trip.TripID, ct);

        if (existing is null)
        {
            trip.TripDate = IstClock.Now;
            trip.StartTime = IstClock.Now;
            _context.Trips.Add(trip);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(trip);
        }

        return await _context.SaveChangesAsync(ct) > 0 ? trip.TripID : null;
    }

    private async Task<string> GenerateTripIdAsync(CancellationToken ct)
    {
        var now = IstClock.Now;
        var year = now.Year;
        var month = now.Month;
        var monthName = now.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);

        var conn = _context.Database.GetDbConnection();
        var closeAfter = conn.State != System.Data.ConnectionState.Open;
        if (closeAfter) await conn.OpenAsync(ct);
        try
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = $@"
                IF NOT EXISTS (SELECT 1 FROM Utility.DocumentNumberDetail WHERE DocumentKey = 'TR' AND YearNo = {year})
                    INSERT INTO Utility.DocumentNumberDetail (DocumentKey, YearNo, January, February, March, April, May, June, July, August, September, October, November, December)
                    VALUES ('TR', {year}, '0','0','0','0','0','0','0','0','0','0','0','0');
                DECLARE @s NVARCHAR(20);
                UPDATE Utility.DocumentNumberDetail SET @s = [{monthName}] = CAST(CAST([{monthName}] AS INT) + 1 AS NVARCHAR(20)) WHERE DocumentKey = 'TR' AND YearNo = {year};
                SELECT CAST(@s AS INT);";
            var result = await cmd.ExecuteScalarAsync(ct);
            var seq = Convert.ToInt32(result);
            return $"TR{year % 100:D2}{month:D2}{seq:D5}";
        }
        finally
        {
            if (closeAfter) await conn.CloseAsync();
        }
    }

    public async Task<bool> EndTripAsync(string tripId, decimal tripEndLat, decimal tripEndLong,
        decimal distanceTravelled, decimal tripMinutes, CancellationToken ct = default)
    {
        return await _context.Trips
            .Where(t => t.TripID == tripId && t.EndTime == null)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.EndTime, IstClock.Now)
                .SetProperty(t => t.TripEndLat, tripEndLat)
                .SetProperty(t => t.TripEndLong, tripEndLong)
                .SetProperty(t => t.DistanceTravelled, distanceTravelled)
                .SetProperty(t => t.TripMinutes, tripMinutes), ct) > 0;
    }

    public async Task<bool> UpdateDistanceTravelledAsync(string tripId, decimal distanceTravelled, CancellationToken ct = default)
    {
        return await _context.Trips
            .Where(t => t.TripID == tripId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(t => t.DistanceTravelled, distanceTravelled), ct) > 0;
    }

    public async Task<bool> DeleteAsync(string tripId, CancellationToken ct = default)
    {
        var trip = await _context.Trips
            .FirstOrDefaultAsync(t => t.TripID == tripId, ct);

        if (trip is null) return false;

        _context.Trips.Remove(trip);
        return await _context.SaveChangesAsync(ct) > 0;
    }
}
