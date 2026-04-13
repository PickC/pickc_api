using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.Modules.Master.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Master.Infrastructure.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly MasterDbContext _context;

    public DriverRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<List<Driver>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Drivers
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<Driver?> GetByIdAsync(string driverId, CancellationToken ct = default)
    {
        return await _context.Drivers
            .Include(d => d.Addresses)
            .Include(d => d.Attachments)
            .Include(d => d.BankDetails)
            .Include(d => d.Ratings)
            .FirstOrDefaultAsync(d => d.DriverID == driverId, ct);
    }

    public async Task<List<Driver>> GetByNameAsync(string name, CancellationToken ct = default)
    {
        return await _context.Drivers
            .AsNoTracking()
            .Where(d => d.DriverName.Contains(name))
            .ToListAsync(ct);
    }

    public async Task<bool> SaveAsync(Driver driver, CancellationToken ct = default)
    {
        var existing = await _context.Drivers
            .FirstOrDefaultAsync(d => d.DriverID == driver.DriverID, ct);

        if (existing is null)
        {
            driver.CreatedOn = IstClock.Now;
            driver.ModifiedOn = IstClock.Now;
            _context.Drivers.Add(driver);
        }
        else
        {
            driver.ModifiedOn = IstClock.Now;
            _context.Entry(existing).CurrentValues.SetValues(driver);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(string driverId, CancellationToken ct = default)
    {
        var driver = await _context.Drivers
            .FirstOrDefaultAsync(d => d.DriverID == driverId, ct);

        if (driver is null) return false;

        _context.Drivers.Remove(driver);
        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> UpdateDeviceIdAsync(string driverId, string deviceId, CancellationToken ct = default)
    {
        return await _context.Drivers
            .Where(d => d.DriverID == driverId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(d => d.DeviceID, deviceId), ct) > 0;
    }

    public async Task<bool> UpdatePasswordAsync(string driverId, string newPassword, CancellationToken ct = default)
    {
        return await _context.Drivers
            .Where(d => d.DriverID == driverId)
            .ExecuteUpdateAsync(s =>
                s.SetProperty(d => d.Password, newPassword), ct) > 0;
    }
}
