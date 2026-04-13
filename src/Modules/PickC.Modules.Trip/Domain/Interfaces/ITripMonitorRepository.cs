using PickC.Modules.Trip.Domain.Entities;

namespace PickC.Modules.Trip.Domain.Interfaces;

public interface ITripMonitorRepository
{
    Task<List<TripMonitor>> GetAllAsync(CancellationToken ct = default);
    Task<List<TripMonitor>> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<List<TripMonitor>> GetByDriverIdAsync(string driverId, CancellationToken ct = default);
    Task<bool> SaveAsync(TripMonitor monitor, CancellationToken ct = default);
}
