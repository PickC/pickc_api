using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Domain.Interfaces;

public interface IAddressRepository
{
    Task<List<Address>> GetByLinkIdAsync(string linkId, CancellationToken ct = default);
    Task<Address?> GetByIdAsync(int addressId, CancellationToken ct = default);
    Task<bool> SaveAsync(Address address, CancellationToken ct = default);
    Task<bool> DeleteAsync(int addressId, CancellationToken ct = default);
}
