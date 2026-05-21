namespace PickC.SharedKernel.Notifications;

public interface IBookingNotifier
{
    Task NotifyBookingAcceptedAsync(string bookingNo, string driverId, string vehicleNo, string otp, CancellationToken ct = default);
    Task NotifyDriverReachedPickupAsync(string bookingNo, CancellationToken ct = default);
    Task NotifyBookingCompletedAsync(string bookingNo, CancellationToken ct = default);
    Task NotifyTripEndedToCustomerAsync(string tripId, CancellationToken ct = default);
    Task NotifyBookingCancelledByCustomerAsync(string bookingNo, CancellationToken ct = default);
    Task NotifyBookingCancelledByDriverAsync(string bookingNo, CancellationToken ct = default);
}
