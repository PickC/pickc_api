using Microsoft.EntityFrameworkCore;
using PickC.Modules.Trip.Domain.Entities;
using PickC.Modules.Trip.Domain.Interfaces;
using PickC.Modules.Trip.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Trip.Infrastructure.Repositories;

public class TripMonitorRepository : ITripMonitorRepository
{
    private readonly TripDbContext _context;

    public TripMonitorRepository(TripDbContext context)
    {
        _context = context;
    }

    public async Task<List<TripMonitor>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.TripMonitors
            .AsNoTracking()
            .OrderByDescending(t => t.RefreshDate)
            .ToListAsync(ct);
    }

    public async Task<List<TripMonitor>> GetByTripIdAsync(string tripId, CancellationToken ct = default)
    {
        return await _context.TripMonitors
            .AsNoTracking()
            .Where(t => t.TripID == tripId)
            .OrderByDescending(t => t.RefreshDate)
            .ToListAsync(ct);
    }

    public async Task<List<TripMonitor>> GetByDriverIdAsync(string driverId, CancellationToken ct = default)
    {
        return await _context.TripMonitors
            .AsNoTracking()
            .Where(t => t.DriverID == driverId)
            .OrderByDescending(t => t.RefreshDate)
            .ToListAsync(ct);
    }

    public async Task<bool> SaveAsync(TripMonitor monitor, CancellationToken ct = default)
    {
        var existing = await _context.TripMonitors
            .FirstOrDefaultAsync(t => t.DriverID == monitor.DriverID && t.TripID == monitor.TripID, ct);

        if (existing is null)
        {
            monitor.RefreshDate = IstClock.Now;
            _context.TripMonitors.Add(monitor);
        }
        else
        {
            _context.Entry(existing).CurrentValues.SetValues(monitor);
            existing.RefreshDate = IstClock.Now;
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }
}
