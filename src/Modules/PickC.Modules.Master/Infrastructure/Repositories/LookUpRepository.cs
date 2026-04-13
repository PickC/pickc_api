using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.Modules.Master.Infrastructure.Data;

namespace PickC.Modules.Master.Infrastructure.Repositories;

public class LookUpRepository : ILookUpRepository
{
    private readonly MasterDbContext _context;

    public LookUpRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<List<LookUp>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.LookUps
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<List<LookUp>> GetByCategoryAsync(string category, CancellationToken ct = default)
    {
        return await _context.LookUps
            .AsNoTracking()
            .Where(l => l.LookupCategory == category)
            .ToListAsync(ct);
    }

    public async Task<LookUp?> GetByIdAsync(short lookupId, CancellationToken ct = default)
    {
        return await _context.LookUps
            .FirstOrDefaultAsync(l => l.LookupID == lookupId, ct);
    }

    public async Task<bool> SaveAsync(LookUp lookUp, CancellationToken ct = default)
    {
        var existing = await _context.LookUps
            .FirstOrDefaultAsync(l => l.LookupID == lookUp.LookupID, ct);

        if (existing is null)
            _context.LookUps.Add(lookUp);
        else
            _context.Entry(existing).CurrentValues.SetValues(lookUp);

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(short lookupId, CancellationToken ct = default)
    {
        var lookUp = await _context.LookUps
            .FirstOrDefaultAsync(l => l.LookupID == lookupId, ct);

        if (lookUp is null) return false;

        _context.LookUps.Remove(lookUp);
        return await _context.SaveChangesAsync(ct) > 0;
    }
}
