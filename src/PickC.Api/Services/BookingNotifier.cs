using Microsoft.AspNetCore.SignalR;
using PickC.Api.Hubs;
using PickC.Modules.Booking.Application.Services;
using PickC.Modules.Master.Application.Services;
using PickC.Modules.Notification.Domain.Interfaces;
using PickC.Modules.Trip.Application.Services;
using PickC.SharedKernel.Notifications;

namespace PickC.Api.Services;

public class BookingNotifier : IBookingNotifier
{
    private readonly IHubContext<BookingHub> _bookingHub;
    private readonly IHubContext<TripHub> _tripHub;
    private readonly IPushNotificationService _fcm;
    private readonly IBookingService _bookingService;
    private readonly ICustomerService _customerService;
    private readonly IDriverService _driverService;
    private readonly ITripService _tripService;
    private readonly ILogger<BookingNotifier> _logger;

    public BookingNotifier(
        IHubContext<BookingHub> bookingHub,
        IHubContext<TripHub> tripHub,
        IPushNotificationService fcm,
        IBookingService bookingService,
        ICustomerService customerService,
        IDriverService driverService,
        ITripService tripService,
        ILogger<BookingNotifier> logger)
    {
        _bookingHub = bookingHub;
        _tripHub = tripHub;
        _fcm = fcm;
        _bookingService = bookingService;
        _customerService = customerService;
        _driverService = driverService;
        _tripService = tripService;
        _logger = logger;
    }

    // ── Step 2: Driver accepts booking → notify customer ──────────────────
    public async Task NotifyBookingAcceptedAsync(
        string bookingNo, string driverId, string vehicleNo, string otp, CancellationToken ct = default)
    {
        try
        {
            var booking = await _bookingService.GetByBookingNoAsync(bookingNo, ct);
            var driver = await _driverService.GetByIdAsync(driverId, ct);

            var payload = new
            {
                bookingNo,
                driverName = driver.DriverName,
                driverMobile = driver.MobileNo,
                driverId = driver.DriverID,
                vehicleNo,
                otp,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            // SignalR → customer watching this booking
            await _bookingHub.Clients.Group($"booking_{bookingNo}")
                .SendAsync("BookingAccepted", payload, ct);

            // FCM → customer device
            await SendFcmToCustomerAsync(booking.CustomerID, bookingNo,
                $"Driver {driver.DriverName} accepted your booking. OTP: {otp}");

            _logger.LogInformation("BookingAccepted sent: booking={BookingNo}, driver={DriverId}", bookingNo, driverId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send BookingAccepted notification for {BookingNo}", bookingNo);
        }
    }

    // ── Step 3: Driver reached pickup → notify customer ───────────────────
    public async Task NotifyDriverReachedPickupAsync(string bookingNo, CancellationToken ct = default)
    {
        try
        {
            var booking = await _bookingService.GetByBookingNoAsync(bookingNo, ct);

            var payload = new
            {
                bookingNo,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            await _bookingHub.Clients.Group($"booking_{bookingNo}")
                .SendAsync("DriverReachedPickup", payload, ct);

            await SendFcmToCustomerAsync(booking.CustomerID, bookingNo,
                "Your driver has arrived at the pickup location.");

            _logger.LogInformation("DriverReachedPickup sent: booking={BookingNo}", bookingNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send DriverReachedPickup notification for {BookingNo}", bookingNo);
        }
    }

    // ── Step 4: Booking completed → notify customer ───────────────────────
    public async Task NotifyBookingCompletedAsync(string bookingNo, CancellationToken ct = default)
    {
        try
        {
            var booking = await _bookingService.GetByBookingNoAsync(bookingNo, ct);

            var payload = new
            {
                bookingNo,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            await _bookingHub.Clients.Group($"booking_{bookingNo}")
                .SendAsync("BookingCompleted", payload, ct);

            await SendFcmToCustomerAsync(booking.CustomerID, bookingNo,
                "Your trip has been completed. Thank you for using PickC!");

            _logger.LogInformation("BookingCompleted sent: booking={BookingNo}", bookingNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send BookingCompleted notification for {BookingNo}", bookingNo);
        }
    }

    // ── Trip ended → notify customer via TripHub + FCM ────────────────────
    public async Task NotifyTripEndedToCustomerAsync(string tripId, CancellationToken ct = default)
    {
        try
        {
            // Push TripEnded to trip group (customers watching via TripHub)
            await _tripHub.Clients.Group($"trip_{tripId}")
                .SendAsync("TripEnded", new { tripId }, ct);

            // Also send FCM
            var trips = await _tripService.GetByTripIdAsync(tripId, ct);
            if (!string.IsNullOrEmpty(trips.CustomerMobile))
            {
                await SendFcmToCustomerAsync(trips.CustomerMobile, trips.BookingNo ?? tripId,
                    "Your trip has ended. Thank you for using PickC!");
            }

            _logger.LogInformation("TripEnded sent: trip={TripId}", tripId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send TripEnded notification for {TripId}", tripId);
        }
    }

    // ── Customer cancels → notify driver ──────────────────────────────────
    public async Task NotifyBookingCancelledByCustomerAsync(string bookingNo, CancellationToken ct = default)
    {
        try
        {
            var payload = new
            {
                bookingNo,
                cancelledBy = "CUSTOMER",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            // SignalR → driver watching this booking
            await _bookingHub.Clients.Group($"booking_driver_{bookingNo}")
                .SendAsync("BookingCancelled", payload, ct);

            _logger.LogInformation("BookingCancelled (by customer) sent: booking={BookingNo}", bookingNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send BookingCancelled notification for {BookingNo}", bookingNo);
        }
    }

    // ── Driver cancels → notify customer ──────────────────────────────────
    public async Task NotifyBookingCancelledByDriverAsync(string bookingNo, CancellationToken ct = default)
    {
        try
        {
            var booking = await _bookingService.GetByBookingNoAsync(bookingNo, ct);

            var payload = new
            {
                bookingNo,
                cancelledBy = "DRIVER",
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            await _bookingHub.Clients.Group($"booking_{bookingNo}")
                .SendAsync("BookingCancelled", payload, ct);

            await SendFcmToCustomerAsync(booking.CustomerID, bookingNo,
                "Your booking has been cancelled by the driver. Please book again.");

            _logger.LogInformation("BookingCancelled (by driver) sent: booking={BookingNo}", bookingNo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send BookingCancelledByDriver notification for {BookingNo}", bookingNo);
        }
    }

    // ── Helper: send FCM to customer by mobile number ─────────────────────
    private async Task SendFcmToCustomerAsync(string customerMobile, string bookingNo, string message)
    {
        try
        {
            var customer = await _customerService.GetByMobileAsync(customerMobile);
            if (!string.IsNullOrEmpty(customer.DeviceID))
            {
                await _fcm.SendAsync(customer.DeviceID, bookingNo, message);
            }
            else
            {
                _logger.LogWarning("Customer {Mobile} has no device token. FCM skipped.", customerMobile);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "FCM to customer {Mobile} failed. Continuing.", customerMobile);
        }
    }
}
