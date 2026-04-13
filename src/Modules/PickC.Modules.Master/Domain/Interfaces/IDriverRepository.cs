using PickC.Modules.Master.Domain.Entities;

namespace PickC.Modules.Master.Domain.Interfaces;

public interface IDriverRepository
{
    Task<List<Driver>> GetAllAsync(CancellationToken ct = default);
    Task<Driver?> GetByIdAsync(string driverId, CancellationToken ct = default);
    Task<List<Driver>> GetByNameAsync(string name, CancellationToken ct = default);
    Task<bool> SaveAsync(Driver driver, CancellationToken ct = default);
    Task<bool> DeleteAsync(string driverId, CancellationToken ct = default);
    Task<bool> UpdateDeviceIdAsync(string driverId, string deviceId, CancellationToken ct = default);
    Task<bool> UpdatePasswordAsync(string driverId, string newPassword, CancellationToken ct = default);
}
