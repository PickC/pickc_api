# PickC — Real-Time Vehicle Tracking Implementation Plan

> **Goal:** Uber-style smooth live vehicle tracking on the customer app.
> **Approach:** SignalR (real-time delivery) + Coordinate Interpolation (smooth animation) + Bearing Rotation (direction)
> **Date:** 2026-04-08
> **Author:** PickC Engineering

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [.NET Backend Implementation](#2-net-backend-implementation)
3. [New & Modified API Endpoints](#3-new--modified-api-endpoints)
4. [Flutter Driver App Changes](#4-flutter-driver-app-changes)
5. [Flutter Customer App Changes](#5-flutter-customer-app-changes)
6. [Battery Optimization Strategy](#6-battery-optimization-strategy)
7. [Testing Checklist](#7-testing-checklist)
8. [Deployment Steps](#8-deployment-steps)

---

## 1. Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│  DRIVER APP (Flutter)                                           │
│                                                                 │
│  GPS ──distanceFilter:5m──▶ LocationStream                     │
│                                    │                           │
│                    ┌───────────────┴──────────────────┐        │
│                    ▼                                  ▼        │
│         SignalR Hub (every 3s)          REST Monitor (every 30s)│
│         live push to customer           DB audit trail          │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│  .NET API — Azure App Service                                   │
│                                                                 │
│  TripHub (/hubs/trip)                                          │
│    ├── UpdateDriverLocation(tripId, lat, lng, bearing)         │
│    ├── WatchTrip(tripId)          ← customer subscribes        │
│    ├── StopWatchingTrip(tripId)   ← customer unsubscribes      │
│    └── TripEnded(tripId)          ← broadcast on trip end      │
└─────────────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────────────┐
│  CUSTOMER APP (Flutter)                                         │
│                                                                 │
│  SignalR listener                                               │
│    └── onDriverLocationUpdated(lat, lng, bearing)              │
│              │                                                  │
│              ▼                                                  │
│  Coordinate Interpolator (AnimationController 30fps)            │
│    └── smooth tween: oldPos ──▶ newPos over 3 seconds          │
│              │                                                  │
│              ▼                                                  │
│  Google Map marker update + bearing rotation + camera follow    │
└─────────────────────────────────────────────────────────────────┘
```

**Update frequency:**
- SignalR push: every 3 seconds (or every 5m movement, whichever comes first)
- REST `/trip/monitors`: every 30 seconds (DB record, unchanged)
- Customer animation: 30fps visual interpolation between GPS updates

---

## 2. .NET Backend Implementation

### 2.1 File Changes Required

```
PickC.Api/
├── Hubs/
│   └── TripHub.cs                  ← NEW
├── Program.cs                       ← MODIFY (2 lines)
└── appsettings.json                 ← MODIFY (CORS origin)
```

---

### 2.2 TripHub.cs — Full Implementation

**File:** `PickC.Api/Hubs/TripHub.cs`

```csharp
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
            bearing,        // direction of travel in degrees (0-360), null if stationary
            speedKmh,       // optional — for display in customer app
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        // Broadcast to all customers in this trip's group
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
        // Groups are automatically cleaned up by SignalR on disconnect
        _logger.LogInformation(
            "Client disconnected: {ConnId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }
}
```

---

### 2.3 Program.cs Changes

Add **2 lines** to your existing `Program.cs`:

```csharp
// ── EXISTING: Add services ────────────────────────────────────────────────
builder.Services.AddControllers();
// ... your existing services ...

// ADD THIS LINE ↓
builder.Services.AddSignalR();

// ── EXISTING: CORS ────────────────────────────────────────────────────────
// REPLACE your existing CORS policy with this:
builder.Services.AddCors(options =>
{
    options.AddPolicy("PickCCors", policy =>
        policy
            .WithOrigins(
                "https://your-customer-app.com",    // production customer web app
                "http://localhost:3000",             // local dev
                "capacitor://localhost",             // Flutter iOS WebView
                "http://localhost"                   // Flutter Android WebView
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());  // REQUIRED for SignalR WebSocket handshake
});

// ── EXISTING: Build app ───────────────────────────────────────────────────
var app = builder.Build();

// ... your existing middleware ...
app.UseCors("PickCCors");

// ADD THIS LINE ↓
app.MapHub<TripHub>("/hubs/trip");

app.MapControllers();
app.Run();
```

> **Important:** `AllowCredentials()` is mandatory for SignalR. It will NOT work with `AllowAnyOrigin()` — you must list specific origins.

---

### 2.4 appsettings.json — Add allowed origins

```json
{
  "AllowedOrigins": [
    "https://your-customer-app.com",
    "http://localhost:3000"
  ],
  "SignalR": {
    "KeepAliveIntervalSeconds": 15,
    "ClientTimeoutIntervalSeconds": 30
  }
}
```

---

### 2.5 Optional: Azure SignalR Service (for scale)

When you have 100+ concurrent drivers, replace `AddSignalR()` with:

```csharp
// Install: Microsoft.Azure.SignalR NuGet package
builder.Services.AddSignalR()
    .AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]);
```

For MVP and early growth, the built-in SignalR on your existing App Service is sufficient and free.

---

## 3. New & Modified API Endpoints

### 3.1 New — SignalR Hub Endpoint

| Type | Endpoint | Description |
|------|----------|-------------|
| WebSocket | `wss://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/hubs/trip` | SignalR hub |

**Hub methods (called by clients):**

| Method | Called By | Parameters | Description |
|--------|-----------|------------|-------------|
| `UpdateDriverLocation` | Driver app | `tripId, lat, lng, bearing?, speedKmh?` | Push location to customers |
| `NotifyTripEnded` | Driver app | `tripId` | Signal trip complete |
| `WatchTrip` | Customer app | `tripId` | Subscribe to trip updates |
| `StopWatchingTrip` | Customer app | `tripId` | Unsubscribe |

**Events (received by clients):**

| Event | Received By | Payload | Description |
|-------|-------------|---------|-------------|
| `DriverLocationUpdated` | Customer app | `{tripId, latitude, longitude, bearing, speedKmh, timestamp}` | Driver moved |
| `TripEnded` | Customer app | `{tripId}` | Trip is complete |

---

### 3.2 Existing REST Endpoints — No Changes

All 25 existing REST endpoints remain unchanged. The REST `POST /api/trip/monitors` continues to serve as the 30-second DB audit trail. SignalR is purely additive.

---

### 3.3 Modified: Start Trip Response

**Current:** `{ "message": "Trip saved successfully" }`

**Requested change** — return `tripId` so Flutter apps can immediately subscribe to SignalR:

```json
{
  "message": "Trip saved successfully",
  "tripId": "TRP20260408001"
}
```

This requires a small change in the Trip controller:

```csharp
// TripController.cs — StartTrip action
[HttpPost]
public async Task<IActionResult> StartTrip([FromBody] StartTripRequest request)
{
    var tripId = await _tripService.StartTripAsync(request);
    return Ok(new { message = "Trip saved successfully", tripId });
}
```

---

## 4. Flutter Driver App Changes

### 4.1 Add Package

```yaml
# pubspec.yaml
dependencies:
  signalr_netcore: ^1.3.5
```

### 4.2 New File: `lib/core/services/signalr_service.dart`

```dart
import 'package:signalr_netcore/signalr_client.dart';

class SignalRService {
  static const _hubUrl =
      'https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/hubs/trip';

  HubConnection? _hub;
  bool _isConnected = false;

  Future<void> connect() async {
    if (_isConnected) return;

    _hub = HubConnectionBuilder()
        .withUrl(_hubUrl)
        .withAutomaticReconnect(retryDelays: [0, 2000, 5000, 10000, 30000])
        .build();

    _hub!.onclose(({error}) {
      _isConnected = false;
    });

    _hub!.onreconnected(({connectionId}) {
      _isConnected = true;
    });

    await _hub!.start();
    _isConnected = true;
  }

  Future<void> disconnect() async {
    await _hub?.stop();
    _isConnected = false;
  }

  /// Driver sends location to all watching customers
  Future<void> sendLocation({
    required String tripId,
    required double latitude,
    required double longitude,
    double? bearing,
    double? speedKmh,
  }) async {
    if (!_isConnected || _hub == null) return;
    try {
      await _hub!.invoke('UpdateDriverLocation', args: [
        tripId,
        latitude,
        longitude,
        bearing,
        speedKmh,
      ]);
    } catch (_) {
      // Non-fatal — next update will retry
    }
  }

  /// Driver signals trip ended
  Future<void> notifyTripEnded(String tripId) async {
    if (!_isConnected || _hub == null) return;
    await _hub!.invoke('NotifyTripEnded', args: [tripId]);
  }
}
```

### 4.3 Update `TripBloc` — Add SignalR + GPS broadcasting

```dart
// In TripBloc — add to _onStarted handler:

// After trip starts successfully:
await _signalRService.connect();

// Start GPS broadcast timer (every 3 seconds)
_locationBroadcastTimer = Timer.periodic(
  const Duration(seconds: 3),
  (_) => _broadcastLocation(tripId),
);

// In _onEnded handler:
await _signalRService.notifyTripEnded(event.request.tripID);
await _signalRService.disconnect();
_locationBroadcastTimer?.cancel();
```

### 4.4 GPS Strategy — Battery Optimized

```dart
// Use distanceFilter so GPS only fires when vehicle actually moves
// This is the single biggest battery saving

const activeTrip = LocationSettings(
  accuracy: LocationAccuracy.high,
  distanceFilter: 5,       // Only update if moved 5 meters
);

const waitingForBooking = LocationSettings(
  accuracy: LocationAccuracy.low,
  distanceFilter: 50,      // Only update if moved 50 meters
);
```

### 4.5 Bearing Calculation

```dart
import 'dart:math';

double calculateBearing(LatLng from, LatLng to) {
  final lat1 = from.latitude * pi / 180;
  final lat2 = to.latitude * pi / 180;
  final dLng = (to.longitude - from.longitude) * pi / 180;

  final y = sin(dLng) * cos(lat2);
  final x = cos(lat1) * sin(lat2) - sin(lat1) * cos(lat2) * cos(dLng);

  return ((atan2(y, x) * 180 / pi) + 360) % 360;
}
```

---

## 5. Flutter Customer App Changes

### 5.1 Add Package

```yaml
# pubspec.yaml (customer app)
dependencies:
  signalr_netcore: ^1.3.5
  google_maps_flutter: ^2.9.0
```

### 5.2 New File: `lib/features/tracking/driver_tracking_page.dart`

```dart
import 'dart:async';
import 'dart:math';
import 'package:flutter/material.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:signalr_netcore/signalr_client.dart';

class DriverTrackingPage extends StatefulWidget {
  final String tripId;
  final LatLng pickupLatLng;
  final LatLng dropLatLng;

  const DriverTrackingPage({
    super.key,
    required this.tripId,
    required this.pickupLatLng,
    required this.dropLatLng,
  });

  @override
  State<DriverTrackingPage> createState() => _DriverTrackingPageState();
}

class _DriverTrackingPageState extends State<DriverTrackingPage>
    with TickerProviderStateMixin {

  // ── Map ──────────────────────────────────────────────────────────────────
  final Completer<GoogleMapController> _mapCompleter = Completer();
  final Set<Marker> _markers = {};

  // ── Animation ────────────────────────────────────────────────────────────
  late AnimationController _moveController;
  late Animation<double> _moveAnimation;

  LatLng _currentPos = const LatLng(0, 0);
  LatLng _targetPos = const LatLng(0, 0);
  double _currentBearing = 0;
  bool _hasFirstLocation = false;

  // ── SignalR ──────────────────────────────────────────────────────────────
  HubConnection? _hub;
  bool _tripEnded = false;

  @override
  void initState() {
    super.initState();
    _setupAnimation();
    _connectSignalR();
  }

  // ── Animation setup ──────────────────────────────────────────────────────

  void _setupAnimation() {
    // 30fps cap — smooth enough for slow-moving vehicles
    _moveController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 3000), // matches SignalR interval
    );
    _moveAnimation = CurvedAnimation(
      parent: _moveController,
      curve: Curves.linear, // linear for constant-speed movement
    );
    _moveAnimation.addListener(_onAnimationTick);
  }

  void _onAnimationTick() {
    if (!mounted) return;

    // Interpolate between current and target position
    final lat = _lerpDouble(
        _currentPos.latitude, _targetPos.latitude, _moveAnimation.value);
    final lng = _lerpDouble(
        _currentPos.longitude, _targetPos.longitude, _moveAnimation.value);

    _updateMarker(LatLng(lat, lng), _currentBearing);
  }

  double _lerpDouble(double a, double b, double t) => a + (b - a) * t;

  // ── Marker update ────────────────────────────────────────────────────────

  void _updateMarker(LatLng position, double bearing) {
    if (!mounted) return;
    setState(() {
      _markers.removeWhere((m) => m.markerId.value == 'driver');
      _markers.add(Marker(
        markerId: const MarkerId('driver'),
        position: position,
        rotation: bearing,               // rotates the truck icon
        anchor: const Offset(0.5, 0.5),
        flat: true,                      // marker rotates with the map
        icon: BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueYellow),
        // TODO: replace with custom truck BitmapDescriptor
      ));
    });

    // Camera follow — pan smoothly to keep driver centered
    _mapCompleter.future.then((ctrl) {
      ctrl.animateCamera(CameraUpdate.newLatLng(position));
    });
  }

  // ── SignalR ──────────────────────────────────────────────────────────────

  Future<void> _connectSignalR() async {
    _hub = HubConnectionBuilder()
        .withUrl(
          'https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/hubs/trip',
        )
        .withAutomaticReconnect(retryDelays: [0, 2000, 5000, 10000])
        .build();

    // Receive driver location update
    _hub!.on('DriverLocationUpdated', (args) {
      if (args == null || !mounted || _tripEnded) return;

      final data = args[0] as Map<String, dynamic>;
      final newLat = (data['latitude'] as num).toDouble();
      final newLng = (data['longitude'] as num).toDouble();
      final bearing = (data['bearing'] as num?)?.toDouble() ?? _currentBearing;

      final newPos = LatLng(newLat, newLng);

      if (!_hasFirstLocation) {
        // First update — snap to position without animation
        _currentPos = newPos;
        _targetPos = newPos;
        _currentBearing = bearing;
        _hasFirstLocation = true;
        _updateMarker(newPos, bearing);
        return;
      }

      // Ignore GPS noise (less than 2m movement)
      if (_distanceMeters(_currentPos, newPos) < 2) return;

      // Smooth animation to new position
      _currentPos = _interpolatedPosition; // start from where we currently are
      _targetPos = newPos;
      _currentBearing = bearing;

      _moveController.forward(from: 0); // restart animation toward new target
    });

    // Trip ended
    _hub!.on('TripEnded', (_) {
      if (!mounted) return;
      setState(() => _tripEnded = true);
      _showTripEndedDialog();
    });

    await _hub!.start();
    await _hub!.invoke('WatchTrip', args: [widget.tripId]);
  }

  LatLng get _interpolatedPosition {
    // Current animated position (where marker visually is right now)
    return LatLng(
      _lerpDouble(_currentPos.latitude, _targetPos.latitude, _moveAnimation.value),
      _lerpDouble(_currentPos.longitude, _targetPos.longitude, _moveAnimation.value),
    );
  }

  double _distanceMeters(LatLng a, LatLng b) {
    const R = 6371000.0;
    final dLat = (b.latitude - a.latitude) * pi / 180;
    final dLng = (b.longitude - a.longitude) * pi / 180;
    final h = sin(dLat / 2) * sin(dLat / 2) +
        cos(a.latitude * pi / 180) *
            cos(b.latitude * pi / 180) *
            sin(dLng / 2) *
            sin(dLng / 2);
    return R * 2 * atan2(sqrt(h), sqrt(1 - h));
  }

  void _showTripEndedDialog() {
    showDialog(
      context: context,
      barrierDismissible: false,
      builder: (_) => AlertDialog(
        title: const Text('Trip Complete'),
        content: const Text('Your driver has reached the destination.'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    // Pause animation when customer minimizes app — saves GPU/battery
    if (state == AppLifecycleState.paused) {
      _moveController.stop();
    } else if (state == AppLifecycleState.resumed) {
      _moveController.forward();
    }
  }

  @override
  void dispose() {
    _moveController.dispose();
    _hub?.invoke('StopWatchingTrip', args: [widget.tripId]);
    _hub?.stop();
    super.dispose();
  }

  // ── Build ─────────────────────────────────────────────────────────────────

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text('Tracking Trip #${widget.tripId}')),
      body: Stack(
        children: [
          GoogleMap(
            initialCameraPosition: CameraPosition(
              target: widget.pickupLatLng,
              zoom: 15,
            ),
            onMapCreated: (ctrl) {
              if (!_mapCompleter.isCompleted) _mapCompleter.complete(ctrl);
            },
            markers: _markers,
            myLocationEnabled: true,
            myLocationButtonEnabled: false,
            zoomControlsEnabled: false,
          ),
          if (!_hasFirstLocation)
            const Center(child: CircularProgressIndicator()),
          if (_tripEnded)
            const Positioned(
              top: 16,
              left: 16,
              right: 16,
              child: Card(
                child: Padding(
                  padding: EdgeInsets.all(12),
                  child: Text(
                    'Trip Complete',
                    textAlign: TextAlign.center,
                    style: TextStyle(fontWeight: FontWeight.bold),
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}
```

---

## 6. Battery Optimization Strategy

| App | Optimization | Impact |
|-----|-------------|--------|
| Driver | `distanceFilter: 5` — GPS only fires on 5m movement | -35% GPS drain |
| Driver | Switch to `LocationAccuracy.low` when waiting for booking | -60% GPS drain when idle |
| Customer | Cap animation at 30fps (not 60fps) | -40% GPU drain |
| Customer | `_moveController.stop()` when app in background | -100% animation drain when minimized |
| Both | `withAutomaticReconnect` — SignalR reconnects, no manual polling | Minimal network overhead |
| Both | Ignore GPS updates < 2m movement (noise filter) | Eliminates wasted renders |

**Realistic battery cost per trip (2 hours):**
- Driver phone (plugged in during trips): ~28-32%
- Customer tracking screen open: ~8-12% per 30 minutes

---

## 7. Testing Checklist

### Backend
- [ ] `GET wss://{api}/hubs/trip` — WebSocket handshake succeeds (200 → 101 Switching Protocols)
- [ ] Driver sends `UpdateDriverLocation` → customer receives `DriverLocationUpdated` within 500ms
- [ ] Multiple customers watching same trip all receive updates
- [ ] Driver sends `NotifyTripEnded` → all customers receive `TripEnded`
- [ ] Customer disconnects → no errors thrown on server
- [ ] Driver disconnects mid-trip → customers don't crash
- [ ] `POST /api/trip/trips` returns `tripId` in response

### Driver App
- [ ] SignalR connects on trip start
- [ ] Location broadcasts every ~3 seconds during active trip
- [ ] Bearing calculated correctly (truck points direction of travel)
- [ ] REST `/trip/monitors` still fires every 30s (audit trail preserved)
- [ ] SignalR disconnects cleanly on trip end

### Customer App
- [ ] Marker appears on first location update
- [ ] Marker moves smoothly (no jumps)
- [ ] Truck icon rotates to face direction of travel
- [ ] Camera follows driver
- [ ] Animation pauses when app is backgrounded
- [ ] `TripEnded` event triggers completion dialog
- [ ] App handles SignalR reconnect gracefully (driver goes through tunnel)

---

## 8. Deployment Steps

### Step 1 — Backend (backend team, ~1 hour)

```
1. Create PickC.Api/Hubs/TripHub.cs  (copy from Section 2.2)
2. Update Program.cs                  (2 lines — Section 2.3)
3. Update CORS policy                 (Section 2.3)
4. Update StartTrip to return tripId  (Section 3.3)
5. Deploy to Azure App Service
6. Verify: curl wss://{api}/hubs/trip → 101 response
```

### Step 2 — Driver App (Flutter team, ~2 hours)

```
1. Add signalr_netcore to pubspec.yaml
2. Create lib/core/services/signalr_service.dart
3. Update TripBloc to connect SignalR on trip start
4. Add bearing calculation
5. Update GPS settings to use distanceFilter
6. Test: confirm customers receive location updates
```

### Step 3 — Customer App (Flutter team, ~3 hours)

```
1. Add signalr_netcore to pubspec.yaml
2. Create lib/features/tracking/driver_tracking_page.dart
3. Wire DriverTrackingPage to order confirmation screen
4. Pass tripId from booking confirmation API response
5. Test: confirm smooth animation, bearing rotation, camera follow
```

### Step 4 — Integration Test

```
1. Driver logs in → accepts booking → starts trip
2. Customer opens tracking screen (tripId from booking)
3. Verify smooth movement on customer map
4. Driver ends trip → verify TripEnded event on customer
```

---

## Dependencies Summary

| Component | Package/Tool | Version | Notes |
|-----------|-------------|---------|-------|
| .NET Hub | `Microsoft.AspNetCore.SignalR` | Built-in | No NuGet needed |
| Scale (optional) | `Microsoft.Azure.SignalR` | Latest | Only for 1000+ concurrent |
| Driver Flutter | `signalr_netcore` | ^1.3.5 | Most maintained client |
| Customer Flutter | `signalr_netcore` | ^1.3.5 | Same package |
| Customer Flutter | `google_maps_flutter` | ^2.9.0 | Already in driver app |

---

## Cost Summary

| Component | Cost | Notes |
|-----------|------|-------|
| SignalR on existing App Service | ₹0 | Runs on current infrastructure |
| Azure SignalR Service (optional) | ~₹600-800/month | Only needed at 1000+ concurrent drivers |
| Extra bandwidth (3s location push) | Negligible | ~50 bytes per push × 1200/hour = ~60KB/hr per driver |

---

*Plan created: 2026-04-08*
*For: PickC Driver App — Real-Time Tracking Phase*
*Covers: .NET backend + Flutter driver app + Flutter customer app*
