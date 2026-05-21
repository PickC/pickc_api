using Microsoft.AspNetCore.SignalR;

namespace PickC.Api.Hubs;

public class BookingHub : Hub
{
    private readonly ILogger<BookingHub> _logger;

    public BookingHub(ILogger<BookingHub> logger)
    {
        _logger = logger;
    }

    // ── Called by CUSTOMER APP after creating a booking ────────────────────
    public async Task WatchBooking(string bookingNo)
    {
        if (string.IsNullOrWhiteSpace(bookingNo)) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"booking_{bookingNo}");
        _logger.LogInformation("Client {ConnId} watching booking {BookingNo}", Context.ConnectionId, bookingNo);
    }

    // ── Called by DRIVER APP after accepting a booking ─────────────────────
    public async Task WatchBookingAsDriver(string bookingNo)
    {
        if (string.IsNullOrWhiteSpace(bookingNo)) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"booking_driver_{bookingNo}");
        _logger.LogInformation("Driver {ConnId} watching booking {BookingNo}", Context.ConnectionId, bookingNo);
    }

    public async Task StopWatchingBooking(string bookingNo)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"booking_{bookingNo}");
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"booking_driver_{bookingNo}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("BookingHub client disconnected: {ConnId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
