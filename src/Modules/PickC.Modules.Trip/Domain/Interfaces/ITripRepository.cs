namespace PickC.Modules.Trip.Domain.Interfaces;

public interface ITripRepository
{
    Task<List<Domain.Entities.Trip>> GetAllAsync(CancellationToken ct = default);
    Task<Domain.Entities.Trip?> GetByTripIdAsync(string tripId, CancellationToken ct = default);
    Task<Domain.Entities.Trip?> GetCurrentByDriverAsync(string driverId, CancellationToken ct = default);
    Task<Domain.Entities.Trip?> GetCurrentByCustomerAsync(string customerMobile, CancellationToken ct = default);
    Task<List<Domain.Entities.Trip>> GetByDriverAsync(string driverId, CancellationToken ct = default);
    Task<List<Domain.Entities.Trip>> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<string?> SaveAsync(Domain.Entities.Trip trip, CancellationToken ct = default);
    Task<bool> EndTripAsync(string tripId, decimal tripEndLat, decimal tripEndLong, decimal distanceTravelled, decimal tripMinutes, CancellationToken ct = default);
    Task<bool> UpdateDistanceTravelledAsync(string tripId, decimal distanceTravelled, CancellationToken ct = default);
    Task<bool> DeleteAsync(string tripId, CancellationToken ct = default);
}
