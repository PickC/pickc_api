using PickC.Modules.Booking.Domain.Entities;

namespace PickC.Modules.Booking.Domain.Interfaces;

public interface IBookingRepository
{
    Task<List<Domain.Entities.Booking>> GetAllAsync(CancellationToken ct = default);
    Task<Domain.Entities.Booking?> GetByBookingNoAsync(string bookingNo, CancellationToken ct = default);
    Task<List<Domain.Entities.Booking>> GetByCustomerAsync(string customerId, CancellationToken ct = default);
    Task<List<Domain.Entities.Booking>> GetByDriverAsync(string driverId, CancellationToken ct = default);
    Task<string> SaveAsync(Domain.Entities.Booking booking, CancellationToken ct = default);
    Task<bool> DeleteAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> ConfirmAsync(string bookingNo, string driverId, string vehicleNo, CancellationToken ct = default);
    Task<bool> CancelByCustomerAsync(string bookingNo, string cancelRemarks, CancellationToken ct = default);
    Task<bool> CancelByDriverAsync(string bookingNo, string driverId, string cancelRemarks, CancellationToken ct = default);
    Task<bool> CompleteAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> UpdateReachPickUpAsync(string bookingNo, CancellationToken ct = default);
    Task<bool> UpdateReachDestinationAsync(string bookingNo, CancellationToken ct = default);
    Task<List<Domain.Entities.Booking>> SearchByDateAsync(DateTime fromDate, DateTime toDate, CancellationToken ct = default);
    Task<List<Domain.Entities.Booking>> SearchByStatusAsync(int status, CancellationToken ct = default);
    Task<List<Domain.Entities.Booking>> GetNearBookingsForDriverAsync(decimal latitude, decimal longitude, double rangeKm, CancellationToken ct = default);
}
