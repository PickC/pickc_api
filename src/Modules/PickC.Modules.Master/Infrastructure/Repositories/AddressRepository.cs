using Microsoft.EntityFrameworkCore;
using PickC.Modules.Master.Domain.Entities;
using PickC.Modules.Master.Domain.Interfaces;
using PickC.Modules.Master.Infrastructure.Data;
using PickC.SharedKernel.Helpers;

namespace PickC.Modules.Master.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly MasterDbContext _context;

    public AddressRepository(MasterDbContext context)
    {
        _context = context;
    }

    public async Task<List<Address>> GetByLinkIdAsync(string linkId, CancellationToken ct = default)
    {
        return await _context.Addresses
            .AsNoTracking()
            .Where(a => a.AddressLinkID == linkId)
            .OrderBy(a => a.SeqNo)
            .ToListAsync(ct);
    }

    public async Task<Address?> GetByIdAsync(int addressId, CancellationToken ct = default)
    {
        return await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == addressId, ct);
    }

    public async Task<bool> SaveAsync(Address address, CancellationToken ct = default)
    {
        if (address.AddressId == 0)
        {
            address.CreatedOn = IstClock.Now;
            _context.Addresses.Add(address);
        }
        else
        {
            var existing = await _context.Addresses
                .FirstOrDefaultAsync(a => a.AddressId == address.AddressId, ct);

            if (existing is null) return false;

            address.ModifiedOn = IstClock.Now;
            _context.Entry(existing).CurrentValues.SetValues(address);
        }

        return await _context.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(int addressId, CancellationToken ct = default)
    {
        var address = await _context.Addresses
            .FirstOrDefaultAsync(a => a.AddressId == addressId, ct);

        if (address is null) return false;

        _context.Addresses.Remove(address);
        return await _context.SaveChangesAsync(ct) > 0;
    }
}
