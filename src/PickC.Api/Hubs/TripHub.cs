using Microsoft.AspNetCore.SignalR;

namespace PickC.Api.Hubs;

/// <summary>
/// Real-time trip tracking hub.
/// Driver app calls UpdateDriverLocation every ~3 seconds.
/// Customer app subscribes via WatchTrip and receives DriverLocationUpdated events.
/// </summary>
public class TripHub : Hub
{
    private readonly ILogger<TripHub> _logger;

    public TripHub(ILogger<TripHub> logger)
    {
        _logger = logger;
    }

    // ── Called by DRIVER APP ─────────────────────────────────────────────────

    /// <summary>
    /// Driver sends current GPS location. Broadcasts to all customers watching this trip.
    /// Called every ~3 seconds during active trip.
    /// </summary>
    public async Task UpdateDriverLocation(
        string tripId,
        double latitude,
        double longitude,
        double? bearing = null,
        double? speedKmh = null)
    {
        if (string.IsNullOrWhiteSpace(tripId))
        {
            _logger.LogWarning("UpdateDriverLocation called with empty tripId");
            return;
        }

        var payload = new
        {
            tripId,
            latitude,
            longitude,
            bearing,
            speedKmh,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        await Clients.Group($"trip_{tripId}")
            .SendAsync("DriverLocationUpdated", payload);

        _logger.LogDebug(
            "Location broadcast: trip={TripId}, lat={Lat}, lng={Lng}",
            tripId, latitude, longitude);
    }

    /// <summary>
    /// Driver signals trip has ended. Customers can dismiss the tracking screen.
    /// </summary>
    public async Task NotifyTripEnded(string tripId)
    {
        await Clients.Group($"trip_{tripId}")
            .SendAsync("TripEnded", new { tripId });

        _logger.LogInformation("Trip ended broadcast: trip={TripId}", tripId);
    }

    // ── Called by CUSTOMER APP ───────────────────────────────────────────────

    /// <summary>
    /// Customer subscribes to real-time updates for a specific trip.
    /// </summary>
    public async Task WatchTrip(string tripId)
    {
        if (string.IsNullOrWhiteSpace(tripId)) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, $"trip_{tripId}");

        _logger.LogInformation(
            "Customer {ConnId} watching trip {TripId}",
            Context.ConnectionId, tripId);
    }

    /// <summary>
    /// Customer unsubscribes (e.g. navigates away from tracking screen).
    /// </summary>
    public async Task StopWatchingTrip(string tripId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"trip_{tripId}");
    }

    // ── Connection lifecycle ─────────────────────────────────────────────────

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
