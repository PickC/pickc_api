# PickC API — Final Endpoint Reference

> **Base URL:** `https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net`
> **Auth:** All protected endpoints require `Authorization: Bearer <accessToken>` header
> **SignalR Hub:** `wss://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/hubs/trip`

---

## Table of Contents

1. [Shared — Auth & OTP](#1-shared--auth--otp)
2. [Shared — Reference Data (Lookups)](#2-shared--reference-data-lookups)
3. [Customer App Endpoints](#3-customer-app-endpoints)
4. [Driver App Endpoints](#4-driver-app-endpoints)
5. [SignalR Real-Time Tracking](#5-signalr-real-time-tracking)

---

## 1. Shared — Auth & OTP

*Used by both Customer and Driver apps*

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/otp/send` | None | Send OTP to mobile number |
| POST | `/api/auth/otp/verify` | None | Verify OTP |
| POST | `/api/auth/refresh` | None | Refresh access token |
| POST | `/api/auth/logout` | Bearer | Logout — revokes all refresh tokens |

### POST `/api/auth/otp/send`
```json
{ "mobileNo": "9876543210" }
```

### POST `/api/auth/otp/verify`
```json
{ "mobileNo": "9876543210", "otp": "123456" }
```

### POST `/api/auth/refresh`
```json
{
  "accessToken": "<expired-or-valid-token>",
  "refreshToken": "<refresh-token>"
}
```
**Response:**
```json
{
  "accessToken": "<new-token>",
  "refreshToken": "<new-refresh-token>",
  "expiresIn": 3600,
  "userType": "CUSTOMER",
  "userId": "9876543210"
}
```

---

## 2. Shared — Reference Data (Lookups)

*Used by both apps for dropdowns and configuration*

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/master/lookups/vehicle-types` | Bearer | Vehicle types (Open / Closed) |
| GET | `/api/master/lookups/vehicle-groups` | Bearer | Vehicle groups (Mini / Small / Medium / Large) |
| GET | `/api/master/lookups/cargo-types` | Bearer | Cargo types with images |
| GET | `/api/master/lookups/loading-types` | Bearer | Loading/Unloading options |
| GET | `/api/master/lookups/category/{category}` | Bearer | Any lookup by category name |

### Vehicle Types Response
```json
[
  { "lookupID": 1300, "lookupCode": "Open",   "lookupDescription": "Open",   "lookupCategory": "VehicleType", "image": "" },
  { "lookupID": 1301, "lookupCode": "Closed", "lookupDescription": "Closed", "lookupCategory": "VehicleType", "image": "" }
]
```

### Vehicle Groups Response
```json
[
  { "lookupID": 1000, "lookupCode": "Mini",   "lookupDescription": "Mini",   "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1001, "lookupCode": "Small",  "lookupDescription": "Small",  "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1002, "lookupCode": "Medium", "lookupDescription": "Medium", "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1003, "lookupCode": "Large",  "lookupDescription": "Large",  "lookupCategory": "VehicleGroup", "image": "" }
]
```

### Cargo Types Response
```json
[
  { "lookupID": 1330, "lookupCode": "Industrial",   "lookupDescription": "Industrial Goods",           "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/IndustrialGoods.png" },
  { "lookupID": 1331, "lookupCode": "Perishable",   "lookupDescription": "Vegitables & Fruits",        "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/healthy-groceries.png" },
  { "lookupID": 1340, "lookupCode": "Household",    "lookupDescription": "Household Items",             "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/Household.png" },
  { "lookupID": 1341, "lookupCode": "Fragile",      "lookupDescription": "Fragile Goods",               "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/fragile.png" },
  { "lookupID": 1342, "lookupCode": "Construction", "lookupDescription": "Construction Material",       "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/ConstructionMaterial.png" },
  { "lookupID": 1345, "lookupCode": "Grocey-FMCG",  "lookupDescription": "Grocery Items",              "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/groceries.png" },
  { "lookupID": 1346, "lookupCode": "Electronics",  "lookupDescription": "Electronics/Home Appliances","lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/Electronics.png" },
  { "lookupID": 1347, "lookupCode": "Hotel",        "lookupDescription": "Hotels & Hospitals Goods",   "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/Hotel-Hospital.png" },
  { "lookupID": 1382, "lookupCode": "Pharmacy",     "lookupDescription": "Pharmacy",                   "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/Pharmacy.png" },
  { "lookupID": 1383, "lookupCode": "Liquid Goods", "lookupDescription": "Liquid Goods",               "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/LiquidGoods.png" },
  { "lookupID": 1384, "lookupCode": "Others",       "lookupDescription": "Others",                     "lookupCategory": "CargoType", "image": "https://pickcapi-.../Images/others.png" }
]
```

### Loading Types Response
```json
[
  { "lookupID": 1370, "lookupCode": "LOAD",   "lookupDescription": "LOADING",               "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1371, "lookupCode": "UNLOAD", "lookupDescription": "UNLOADING",             "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1372, "lookupCode": "ALL",    "lookupDescription": "LOADING AND UNLOADING", "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1373, "lookupCode": "NONE",   "lookupDescription": "NONE",                  "lookupCategory": "LoadingUnLoading", "image": "" }
]
```

---

## 3. Customer App Endpoints

### 3.1 Login

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/customer/login` | None | Customer login with mobile + OTP |

### POST `/api/auth/customer/login`
```json
{
  "mobileNo": "9876543210",
  "otp": "123456"
}
```
**Response 200:**
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 3600,
  "userType": "CUSTOMER",
  "userId": "9876543210"
}
```

---

### 3.2 Customer Profile

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/master/customers/{mobileNo}` | Bearer | Get customer profile |
| POST | `/api/master/customers` | Bearer | Create or update customer profile |
| PUT | `/api/master/customers/device` | Bearer | Update FCM device token |
| PUT | `/api/master/customers/password` | Bearer | Update password |

### POST `/api/master/customers` — Create/Update Profile
```json
{
  "mobileNo": "9876543210",
  "customerName": "Ravi Kumar",
  "emailId": "ravi@email.com",
  "cityName": "Hyderabad"
}
```

### PUT `/api/master/customers/device` — Update Device Token
```json
{
  "mobileNo": "9876543210",
  "deviceId": "<fcm-device-token>"
}
```

---

### 3.3 Address Management

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/master/addresses/{linkId}` | Bearer | Get saved addresses by linkId (mobileNo) |
| POST | `/api/master/addresses` | Bearer | Save address |
| DELETE | `/api/master/addresses/{id}` | Bearer | Delete address by ID |

---

### 3.4 Booking (Customer)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/booking/bookings` | Bearer | Create a new booking |
| GET | `/api/booking/bookings/{bookingNo}` | Bearer | Get booking details |
| GET | `/api/booking/bookings/customer/{customerId}` | Bearer | Get all bookings for a customer |
| PUT | `/api/booking/bookings/cancel` | Bearer | Cancel booking (by customer) |

### POST `/api/booking/bookings` — Create Booking
```json
{
  "bookingNo": "",
  "customerID": "9876543210",
  "customerName": "Ravi Kumar",
  "mobileNo": "9876543210",
  "vehicleType": 1300,
  "vehicleGroup": 1000,
  "locationFrom": "12, MG Road, Hyderabad",
  "locationTo": "45, Banjara Hills, Hyderabad",
  "latitude": 17.3850,
  "longitude": 78.4867,
  "destinationLat": 17.4100,
  "destinationLng": 78.5200,
  "distance": 8.5,
  "cargoType": 1340,
  "loadingType": 1372,
  "remarks": ""
}
```
**Response 200:** `{ "message": "Booking saved successfully", "bookingNo": "BK2604000001" }`

### PUT `/api/booking/bookings/cancel` — Cancel Booking
```json
{
  "bookingNo": "BK2604000001",
  "cancelRemarks": "Changed plans"
}
```
**Response 200:** `{ "message": "Booking cancelled" }`

---

### 3.5 Trip Tracking (Customer)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/trip/trips/customer/{customerMobile}/current` | Bearer | Get current active trip for customer |
| GET | `/api/trip/trips/booking/{bookingNo}` | Bearer | Get trip by booking number |

### GET `/api/trip/trips/customer/{customerMobile}/current` — Response 200:
```json
{
  "tripID": "TR2604000001",
  "tripDate": "2026-04-08T10:00:00Z",
  "customerMobile": "9876543210",
  "driverID": "DR260200001",
  "vehicleNo": "TS 00 UC 9999",
  "vehicleType": 1300,
  "vehicleGroup": 1000,
  "locationFrom": "12, MG Road, Hyderabad",
  "locationTo": "45, Banjara Hills, Hyderabad",
  "distance": 8.5,
  "startTime": "2026-04-08T10:00:00Z",
  "endTime": null,
  "distanceTravelled": 4.2,
  "bookingNo": "BK2604000001"
}
```
**Response 404:** `{ "message": "No active trip found" }`

---

### 3.6 Invoice (Customer)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/billing/invoices/booking/{bookingNo}` | Bearer | Get invoice for a booking |
| GET | `/api/billing/invoices/trip/{tripId}` | Bearer | Get invoice for a trip |

### Invoice Response:
```json
{
  "invoiceNo": "IN2604000001",
  "tripID": "TR2604000001",
  "tripAmount": 450.00,
  "taxAmount": 45.00,
  "tipAmount": 20.00,
  "totalAmount": 515.00,
  "paymentType": 1,
  "isPaid": false,
  "bookingNo": "BK2604000001"
}
```
> `paymentType`: `1` = Cash, `2` = Online/UPI, `3` = Wallet

---

### Customer App — Quick Reference

| # | Method | Endpoint | Description |
|---|--------|----------|-------------|
| 1 | POST | `/api/auth/otp/send` | Send OTP |
| 2 | POST | `/api/auth/otp/verify` | Verify OTP |
| 3 | POST | `/api/auth/customer/login` | Login |
| 4 | POST | `/api/auth/refresh` | Refresh token |
| 5 | POST | `/api/auth/logout` | Logout |
| 6 | GET | `/api/master/customers/{mobileNo}` | Get profile |
| 7 | POST | `/api/master/customers` | Create/update profile |
| 8 | PUT | `/api/master/customers/device` | Update device token |
| 9 | GET | `/api/master/addresses/{linkId}` | Get addresses |
| 10 | POST | `/api/master/addresses` | Save address |
| 11 | DELETE | `/api/master/addresses/{id}` | Delete address |
| 12 | GET | `/api/master/lookups/vehicle-types` | Vehicle types |
| 13 | GET | `/api/master/lookups/vehicle-groups` | Vehicle groups |
| 14 | GET | `/api/master/lookups/cargo-types` | Cargo types |
| 15 | GET | `/api/master/lookups/loading-types` | Loading types |
| 16 | POST | `/api/booking/bookings` | Create booking |
| 17 | GET | `/api/booking/bookings/{bookingNo}` | Get booking |
| 18 | GET | `/api/booking/bookings/customer/{customerId}` | My bookings |
| 19 | PUT | `/api/booking/bookings/cancel` | Cancel booking |
| 20 | GET | `/api/trip/trips/customer/{mobile}/current` | Track current trip |
| 21 | GET | `/api/trip/trips/booking/{bookingNo}` | Trip by booking |
| 22 | GET | `/api/billing/invoices/booking/{bookingNo}` | Invoice |
| 23 | WS | `/hubs/trip` → `WatchTrip(tripId)` | Live driver tracking |

---

---

## 4. Driver App Endpoints

### 4.1 Login

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/driver/login` | None | Driver login with driverId + password |

### POST `/api/auth/driver/login`
```json
{
  "driverId": "DR260200001",
  "password": "12345678",
  "latitude": 17.3850,
  "longitude": 78.4867
}
```
**Response 200:**
```json
{
  "accessToken": "<jwt>",
  "refreshToken": "<refresh-token>",
  "expiresIn": 3600,
  "userType": "DRIVER",
  "userId": "DR260200001"
}
```
> Side effect: Sets `IsLogIn=true`, `IsOnDuty=true`, saves login location in `DriverActivity`

---

### 4.2 Driver Profile

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/master/drivers/{driverId}` | Bearer | Get driver profile |
| PUT | `/api/master/drivers/password` | Bearer | Change password |

### GET `/api/master/drivers/DR260200001` — Response 200:
```json
{
  "driverID": "DR260200001",
  "driverName": "Pick-C Supply",
  "vehicleNo": "TS 00 UC 9999",
  "mobileNo": "9659742742",
  "licenseNo": "9659742742",
  "status": true,
  "isVerified": false,
  "gender": 1310,
  "addresses": [],
  "attachments": [],
  "bankDetails": null
}
```

---

### 4.3 Booking (Driver)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/booking/bookings/nearby?lat={lat}&lng={lng}&range={range}` | Bearer | Get unconfirmed bookings near driver's location |
| GET | `/api/booking/bookings/driver/{driverId}` | Bearer | Get all assigned/confirmed bookings for driver |
| GET | `/api/booking/bookings/{bookingNo}` | Bearer | Get booking details |
| PUT | `/api/booking/bookings/confirm` | Bearer | Accept / confirm a booking |
| PUT | `/api/booking/bookings/cancel-by-driver` | Bearer | Cancel a confirmed booking |
| PUT | `/api/booking/bookings/{bookingNo}/reach-pickup` | Bearer | Mark arrived at pickup |
| PUT | `/api/booking/bookings/{bookingNo}/reach-destination` | Bearer | Mark arrived at destination |
| PUT | `/api/booking/bookings/{bookingNo}/complete` | Bearer | Mark booking as complete |

### GET `/api/booking/bookings/nearby` — Query Params
```
lat=17.3850&lng=78.4867&range=15
```
> `range` is in kilometres. Returns unconfirmed, non-cancelled bookings within range.

### PUT `/api/booking/bookings/confirm` — Accept Booking
```json
{
  "bookingNo": "BK2604000001",
  "driverID": "DR260200001",
  "vehicleNo": "TS 00 UC 9999"
}
```
**Response 200:** `{ "message": "Booking confirmed" }`
**Response 400:** `{ "message": "Booking cannot be confirmed" }`

### PUT `/api/booking/bookings/cancel-by-driver`
```json
{
  "bookingNo": "BK2604000001",
  "driverID": "DR260200001",
  "cancelRemarks": "Vehicle breakdown"
}
```
**Response 200:** `{ "message": "Booking cancelled by driver" }`

### PUT `/api/booking/bookings/{bookingNo}/reach-pickup`
- Body: *(empty)*
- **Response 200:** `{ "message": "Reached pickup location" }`

### PUT `/api/booking/bookings/{bookingNo}/reach-destination`
- Body: *(empty)*
- **Response 200:** `{ "message": "Reached destination" }`

### PUT `/api/booking/bookings/{bookingNo}/complete`
- Body: *(empty)*
- **Response 200:** `{ "message": "Booking completed" }`

---

### 4.4 Trip Management (Driver)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/trip/trips` | Bearer | Start a new trip |
| GET | `/api/trip/trips/driver/{driverId}/current` | Bearer | Get current active trip |
| GET | `/api/trip/trips/driver/{driverId}` | Bearer | Get all trips by driver |
| PATCH | `/api/trip/trips/distance` | Bearer | Update distance travelled during trip |
| PUT | `/api/trip/trips/end` | Bearer | End trip |
| DELETE | `/api/trip/trips/{tripId}` | Bearer | Delete trip |

### POST `/api/trip/trips` — Start Trip
```json
{
  "tripID": "",
  "customerMobile": "9876543210",
  "driverID": "DR260200001",
  "vehicleNo": "TS 00 UC 9999",
  "vehicleType": 1300,
  "vehicleGroup": 1000,
  "locationFrom": "12, MG Road, Hyderabad",
  "locationTo": "45, Banjara Hills, Hyderabad",
  "distance": 8.5,
  "waitingMinutes": 5,
  "totalWeight": "500 KG",
  "cargoDescription": "Household goods",
  "remarks": "",
  "latitude": 17.3850,
  "longitude": 78.4867,
  "bookingNo": "BK2604000001"
}
```
**Response 200:**
```json
{
  "message": "Trip saved successfully",
  "tripId": "TR2604000001"
}
```
> `tripId` is auto-generated server-side if `tripID` is sent as empty string.
> Use this `tripId` immediately to connect to SignalR hub for live tracking.

### PATCH `/api/trip/trips/distance` — Update Distance
```json
{
  "tripID": "TR2604000001",
  "distanceTravelled": 4.7
}
```
**Response 200:** `{ "message": "Distance updated" }`

### PUT `/api/trip/trips/end` — End Trip
```json
{
  "tripID": "TR2604000001",
  "tripEndLat": 17.4100,
  "tripEndLong": 78.5200,
  "distanceTravelled": 9.1,
  "tripMinutes": 42
}
```
**Response 200:** `{ "message": "Trip ended successfully" }`

---

### 4.5 GPS Tracking — REST Audit Trail (every 30s)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/trip/monitors` | Bearer | Report GPS location to DB (30s interval) |

### POST `/api/trip/monitors`
```json
{
  "driverID": "DR260200001",
  "tripID": "TR2604000001",
  "vehicleNo": "TS 00 UC 9999",
  "latitude": 17.3850,
  "longitude": 78.4867,
  "tripType": 1,
  "bearing": 45.50,
  "speedKmh": 32.00
}
```
**Response 200:** `{ "message": "Trip monitor saved" }`

> `tripType`: `0` = going to pickup, `1` = in transit (loaded), `2` = returning
> `bearing`: direction of travel in degrees (0–360)
> `speedKmh`: optional current speed

---

### 4.6 Billing (Driver)

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/billing/invoices` | Bearer | Create invoice after trip ends |
| PUT | `/api/billing/invoices/pay` | Bearer | Mark invoice as paid |
| GET | `/api/billing/drivers/{driverId}/invoices` | Bearer | All invoices for driver (accounts list) |
| GET | `/api/billing/drivers/{driverId}/summary` | Bearer | Payment summary (today, balance) |
| GET | `/api/billing/drivers/{driverId}/payments` | Bearer | Full payment history |

### POST `/api/billing/invoices` — Create Invoice
```json
{
  "invoiceNo": "",
  "tripID": "TR2604000001",
  "tripAmount": 450.00,
  "taxAmount": 45.00,
  "tipAmount": 20.00,
  "totalAmount": 515.00,
  "paymentType": 1,
  "bookingNo": "BK2604000001"
}
```
**Response 200:** `{ "message": "Invoice saved successfully" }`

### PUT `/api/billing/invoices/pay` — Mark Paid
```json
{
  "invoiceNo": "IN2604000001",
  "tripID": "TR2604000001",
  "paidAmount": 515.00
}
```
**Response 200:** `{ "message": "Invoice marked as paid" }`

### GET `/api/billing/drivers/{driverId}/invoices` — Accounts List Response:
```json
[
  {
    "invoiceNo": "IN2604000001",
    "tripID": "TR2604000001",
    "bookingNo": "BK2604000001",
    "tripDate": "2026-04-08T10:00:00Z",
    "locationFrom": "12, MG Road, Hyderabad",
    "locationTo": "45, Banjara Hills, Hyderabad",
    "totalAmount": 515.00,
    "paymentType": 1,
    "isPaid": true
  }
]
```
> `paymentType`: `1` = Cash, `2` = Online/UPI, `3` = Wallet
> Results ordered by trip date descending (most recent first)

### GET `/api/billing/drivers/{driverId}/summary` — Response:
```json
{
  "currentBalance": 0.00,
  "todaySummary": 0.00,
  "todayBookings": 0,
  "todayPayment": 0.00,
  "lastPayment": 0.00
}
```

---

### Driver App — Quick Reference

| # | Method | Endpoint | Description |
|---|--------|----------|-------------|
| 1 | POST | `/api/auth/driver/login` | Login |
| 2 | POST | `/api/auth/refresh` | Refresh token |
| 3 | POST | `/api/auth/logout` | Logout |
| 4 | GET | `/api/master/drivers/{driverId}` | Get profile |
| 4a | PUT | `/api/master/drivers/password` | Change password |
| 5 | GET | `/api/master/lookups/vehicle-types` | Vehicle types |
| 6 | GET | `/api/master/lookups/vehicle-groups` | Vehicle groups |
| 7 | GET | `/api/master/lookups/cargo-types` | Cargo types |
| 8 | GET | `/api/master/lookups/loading-types` | Loading types |
| 9 | GET | `/api/booking/bookings/nearby?lat=&lng=&range=` | Nearby bookings |
| 10 | GET | `/api/booking/bookings/driver/{driverId}` | My assigned bookings |
| 11 | GET | `/api/booking/bookings/{bookingNo}` | Booking details |
| 12 | PUT | `/api/booking/bookings/confirm` | Accept booking |
| 13 | PUT | `/api/booking/bookings/cancel-by-driver` | Cancel booking |
| 14 | PUT | `/api/booking/bookings/{bookingNo}/reach-pickup` | Reached pickup |
| 15 | PUT | `/api/booking/bookings/{bookingNo}/reach-destination` | Reached destination |
| 16 | PUT | `/api/booking/bookings/{bookingNo}/complete` | Complete booking |
| 17 | POST | `/api/trip/trips` | Start trip → returns `tripId` |
| 18 | GET | `/api/trip/trips/driver/{driverId}/current` | Current active trip |
| 19 | GET | `/api/trip/trips/driver/{driverId}` | All my trips |
| 20 | PATCH | `/api/trip/trips/distance` | Update distance |
| 21 | PUT | `/api/trip/trips/end` | End trip |
| 22 | POST | `/api/trip/monitors` | GPS audit trail (every 30s) |
| 23 | POST | `/api/billing/invoices` | Create invoice |
| 24 | PUT | `/api/billing/invoices/pay` | Mark invoice paid |
| 25 | GET | `/api/billing/drivers/{driverId}/invoices` | All invoices / accounts list |
| 26 | GET | `/api/billing/drivers/{driverId}/summary` | Payment summary |
| 27 | GET | `/api/billing/drivers/{driverId}/payments` | Payment history |
| 28 | WS | `/hubs/trip` → `UpdateDriverLocation(...)` | Live GPS push |

---

---

## 5. SignalR Real-Time Tracking

**Hub URL:** `wss://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/hubs/trip`

### Methods called by Driver App

| Method | Parameters | Description |
|--------|------------|-------------|
| `UpdateDriverLocation` | `tripId, latitude, longitude, bearing?, speedKmh?` | Push live GPS every ~3 seconds |
| `NotifyTripEnded` | `tripId` | Signal trip is complete |

### Methods called by Customer App

| Method | Parameters | Description |
|--------|------------|-------------|
| `WatchTrip` | `tripId` | Subscribe to live driver location |
| `StopWatchingTrip` | `tripId` | Unsubscribe when leaving tracking screen |

### Events received by Customer App

| Event | Payload | Description |
|-------|---------|-------------|
| `DriverLocationUpdated` | `{ tripId, latitude, longitude, bearing, speedKmh, timestamp }` | Driver moved — update map marker |
| `TripEnded` | `{ tripId }` | Trip complete — dismiss tracking screen |

### Driver App — Flutter Flow
```dart
// 1. Start trip → get tripId from response
final tripId = response['tripId'];

// 2. Connect to SignalR
await signalRService.connect();

// 3. Push location every 3 seconds
Timer.periodic(Duration(seconds: 3), (_) =>
  signalRService.sendLocation(tripId: tripId, lat: lat, lng: lng, bearing: bearing));

// 4. Continue REST audit trail every 30 seconds to /api/trip/monitors

// 5. On trip end
await signalRService.notifyTripEnded(tripId);
await signalRService.disconnect();
```

### Customer App — Flutter Flow
```dart
// 1. Get tripId from current trip or booking
final tripId = currentTrip['tripID'];

// 2. Connect and subscribe
await hub.start();
await hub.invoke('WatchTrip', args: [tripId]);

// 3. Listen for updates
hub.on('DriverLocationUpdated', (args) {
  final lat = args[0]['latitude'];
  final lng = args[0]['longitude'];
  final bearing = args[0]['bearing'];
  // Animate marker on Google Map
});

// 4. Handle trip end
hub.on('TripEnded', (_) => showTripCompleteDialog());
```

---

## Token Details

| Field | Value |
|-------|-------|
| Access token lifetime | 60 minutes |
| Refresh token lifetime | 30 days |
| Token type | JWT Bearer |
| `userType` claim | `CUSTOMER` or `DRIVER` |

---

## Health Check

```
GET https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/health
```
Returns `200 OK` if service is up.

---

*Generated: 2026-04-08 | PickC Core API — Azure Production*
*Base URL: `https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net`*
