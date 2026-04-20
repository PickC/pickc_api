# PickC API — Driver API Testing

> **Test Date:** 2026-04-07
> **Base URL:** `https://localhost:7019` (dev) / `https://pickcapi-atgcb7d4afccanav.centralindia-01.azurewebsites.net/` (prod)
> **Test Driver:** `DR260200001` — Pick-C Supply | Vehicle: `TS 00 UC 9999`
> **Environment:** Development (JWT validation active on login; DevBypass for [Authorize] endpoints)

---

## 1. Driver Login

**POST** `/api/auth/driver/login`

**Request:**
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
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJEUjI2MDIwMDAwMSIsImp0aSI6ImRmZGMxOWYyLWE4Y2UtNDg5MS1hMzJmLTFjOTZiNmYwYmNmMSIsInVzZXJUeXBlIjoiRFJJVkVSIiwibW9iaWxlTm8iOiI5NjU5NzQyNzQyIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiRFJJVkVSIiwiZXhwIjoxNzc1NTc0NTA3LCJpc3MiOiJQaWNrQy5BcGkiLCJhdWQiOiJQaWNrQy5DbGllbnRzIn0.XvkwVPUi7aXBMQaRE9p7mwMLP7WswprK3bVeVy06XHg",
  "refreshToken": "Qv6EwelG4d8p+dx4OKmun+QqwN+mV37eWeBUf4h2rDfDpZ1noi9NOsnVXZgEUKreDIF7b64iAmla7GXduASEsQ==",
  "expiresIn": 3600,
  "userType": "DRIVER",
  "userId": "DR260200001"
}
```

> **Side effects:** Upserts `Operation.DriverActivity` — sets `IsLogIn=true`, `LoginDate`, `IsOnDuty=true`, `Latitude/Longitude`, `VehicleNo`.

**Wrong credentials → 401:**
```json
{ "message": "Invalid credentials" }
```

---

## 2. Get Driver Profile

**GET** `/api/master/drivers/DR260200001`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:**
```json
{
  "driverID": "DR260200001",
  "driverName": "Pick-C Supply",
  "vehicleNo": "TS 00 UC 9999",
  "mobileNo": "9659742742",
  "phoneNo": "9659742742",
  "licenseNo": "9659742742",
  "status": true,
  "operatorID": "OP190500004",
  "isVerified": false,
  "fatherName": "",
  "dateOfBirth": "2000-01-01T00:00:00",
  "placeOfBirth": "HYDERABAD",
  "gender": 1310,
  "maritialStatus": 1321,
  "panNo": "ABCD1234F",
  "aadharCardNo": "none",
  "nationality": "INDIAN",
  "vehicleRCNo": null,
  "addresses": [],
  "attachments": [],
  "bankDetails": null
}
```

---

## 3. Get Nearby Available Bookings

**GET** `/api/booking/bookings/nearby?lat=17.3850&lng=78.4867&range=15`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** *(empty — no unconfirmed bookings near this location during test)*
```json
[]
```

---

## 4. Get Driver's Assigned Bookings

**GET** `/api/booking/bookings/driver/DR260200001`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** *(no bookings assigned to this driver during test)*
```json
[]
```

---

## 5. Accept a Booking (Confirm)

**PUT** `/api/booking/bookings/confirm`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "bookingNo": "BK20260407001",
  "driverID": "DR260200001",
  "vehicleNo": "TS 00 UC 9999"
}
```

**Response 200:**
```json
{ "message": "Booking confirmed" }
```

**Response 400** *(booking not found or already confirmed — tested with dummy booking)*:
```json
{ "message": "Booking cannot be confirmed" }
```

---

## 6. Cancel Booking by Driver

**PUT** `/api/booking/bookings/cancel-by-driver`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "bookingNo": "BK20260407001",
  "driverID": "DR260200001",
  "cancelRemarks": "Vehicle breakdown"
}
```

**Response 200:**
```json
{ "message": "Booking cancelled by driver" }
```

**Response 400:**
```json
{ "message": "Booking cannot be cancelled" }
```

---

## 7. Mark Reached Pickup

**PUT** `/api/booking/bookings/BK20260407001/reach-pickup`
**Header:** `Authorization: Bearer <accessToken>`
**Body:** *(empty)*

**Response 200:**
```json
{ "message": "Reached pickup location" }
```

---

## 8. Mark Reached Destination

**PUT** `/api/booking/bookings/BK20260407001/reach-destination`
**Header:** `Authorization: Bearer <accessToken>`
**Body:** *(empty)*

**Response 200:**
```json
{ "message": "Reached destination" }
```

---

## 9. Complete Booking

**PUT** `/api/booking/bookings/BK20260407001/complete`
**Header:** `Authorization: Bearer <accessToken>`
**Body:** *(empty)*

**Response 200:**
```json
{ "message": "Booking completed" }
```

---

## 10. Get Current Active Trip

**GET** `/api/trip/trips/driver/DR260200001/current`
**Header:** `Authorization: Bearer <accessToken>`

**Response 404** *(no active trip during test)*:
```json
{ "message": "No active trip found" }
```

**Response 200** *(when trip is active)*:
```json
{
  "tripID": "TRP20260407001",
  "tripDate": "2026-04-07T10:00:00Z",
  "customerMobile": "9876543210",
  "driverID": "DR260200001",
  "vehicleNo": "TS 00 UC 9999",
  "vehicleType": 1300,
  "vehicleGroup": 1000,
  "locationFrom": "12, MG Road, Hyderabad",
  "locationTo": "45, Banjara Hills, Hyderabad",
  "distance": 8.5,
  "startTime": "2026-04-07T10:00:00Z",
  "endTime": null,
  "tripMinutes": 0,
  "waitingMinutes": 5,
  "totalWeight": "500 KG",
  "cargoDescription": "Household goods",
  "remarks": "",
  "latitude": 17.3850,
  "longitude": 78.4867,
  "tripEndLat": null,
  "tripEndLong": null,
  "distanceTravelled": 0,
  "bookingNo": "BK20260407001"
}
```

---

## 11. Get All Driver Trips

**GET** `/api/trip/trips/driver/DR260200001`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** *(no trips recorded for this driver during test)*
```json
[]
```

---

## 12. Start Trip

**POST** `/api/trip/trips`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
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
  "bookingNo": "BK20260407001"
}
```

**Response 200:**
```json
{ "message": "Trip saved successfully" }
```

---

## 13. Update Distance During Trip

**PATCH** `/api/trip/trips/distance`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "tripID": "TRP20260407001",
  "distanceTravelled": 4.7
}
```

**Response 200:**
```json
{ "message": "Distance updated" }
```

---

## 14. End Trip

**PUT** `/api/trip/trips/end`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "tripID": "TRP20260407001",
  "tripEndLat": 17.4100,
  "tripEndLong": 78.5200,
  "distanceTravelled": 9.1,
  "tripMinutes": 42
}
```

**Response 200:**
```json
{ "message": "Trip ended successfully" }
```

---

## 15. Report GPS Location (Trip Monitor)

**POST** `/api/trip/monitors`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "driverID": "DR260200001",
  "tripID": "TRP-SAMPLE-001",
  "vehicleNo": "TS 00 UC 9999",
  "latitude": 17.3850,
  "longitude": 78.4867,
  "tripType": 0
}
```

**Response 200:** ✅ *Tested and confirmed*
```json
{ "message": "Trip monitor saved" }
```

> `tripType`: `0` = going to pickup, `1` = in transit (loaded), `2` = returning

---

## 16. Create Invoice

**POST** `/api/billing/invoices`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "invoiceNo": "",
  "tripID": "TRP20260407001",
  "tripAmount": 450.00,
  "taxAmount": 45.00,
  "tipAmount": 20.00,
  "totalAmount": 515.00,
  "paymentType": 1,
  "bookingNo": "BK20260407001"
}
```

**Response 200:**
```json
{ "message": "Invoice saved successfully" }
```

> `paymentType`: `1` = Cash, `2` = Online/UPI, `3` = Wallet

---

## 17. Mark Invoice as Paid

**PUT** `/api/billing/invoices/pay`
**Header:** `Authorization: Bearer <accessToken>`

**Request:**
```json
{
  "invoiceNo": "INV20260407001",
  "tripID": "TRP20260407001",
  "paidAmount": 515.00
}
```

**Response 200:**
```json
{ "message": "Invoice marked as paid" }
```

---

## 18. Get Driver Payment Summary

**GET** `/api/billing/drivers/DR260200001/summary`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
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

## 19. Get Driver Payment History

**GET** `/api/billing/drivers/DR260200001/payments`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
[]
```

---

## 20. Lookup — Vehicle Types

**GET** `/api/master/lookups/vehicle-types`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
[
  { "lookupID": 1300, "lookupCode": "Open",   "lookupDescription": "Open",   "lookupCategory": "VehicleType", "image": "" },
  { "lookupID": 1301, "lookupCode": "Closed", "lookupDescription": "Closed", "lookupCategory": "VehicleType", "image": "" }
]
```

---

## 21. Lookup — Vehicle Groups

**GET** `/api/master/lookups/vehicle-groups`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
[
  { "lookupID": 1000, "lookupCode": "Mini",   "lookupDescription": "Mini",   "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1001, "lookupCode": "Small",  "lookupDescription": "Small",  "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1002, "lookupCode": "Medium", "lookupDescription": "Medium", "lookupCategory": "VehicleGroup", "image": "" },
  { "lookupID": 1003, "lookupCode": "Large",  "lookupDescription": "Large",  "lookupCategory": "VehicleGroup", "image": "" }
]
```

---

## 22. Lookup — Cargo Types

**GET** `/api/master/lookups/cargo-types`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
[
  { "lookupID": 1330, "lookupCode": "Industrial",  "lookupDescription": "Industrial Goods",          "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/IndustrialGoods.png" },
  { "lookupID": 1331, "lookupCode": "Perishable",  "lookupDescription": "Vegitables & Fruits",       "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/healthy-groceries.png" },
  { "lookupID": 1340, "lookupCode": "Household",   "lookupDescription": "Household Items",            "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/Household.png" },
  { "lookupID": 1341, "lookupCode": "Fragile",     "lookupDescription": "Fragile Goods",              "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/fragile.png" },
  { "lookupID": 1342, "lookupCode": "Construction","lookupDescription": "Construction Material",      "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/ConstructionMaterial.png" },
  { "lookupID": 1345, "lookupCode": "Grocey-FMCG", "lookupDescription": "Grocery Items",             "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/groceries.png" },
  { "lookupID": 1346, "lookupCode": "Electronics", "lookupDescription": "Electronics/Home Appliances","lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/Electronics.png" },
  { "lookupID": 1347, "lookupCode": "Hotel",       "lookupDescription": "Hotels & Hospitals Goods",   "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/Hotel-Hospital.png" },
  { "lookupID": 1382, "lookupCode": "Pharmacy",    "lookupDescription": "Pharmacy",                   "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/Pharmacy.png" },
  { "lookupID": 1383, "lookupCode": "Liquid Goods","lookupDescription": "Liquid Goods",               "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/LiquidGoods.png" },
  { "lookupID": 1384, "lookupCode": "Others",      "lookupDescription": "Others",                     "lookupCategory": "CargoType", "image": "http://api.pickcargo.in/Images/others.png" }
]
```

---

## 23. Lookup — Loading/Unloading Types

**GET** `/api/master/lookups/loading-types`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
[
  { "lookupID": 1370, "lookupCode": "LOAD",   "lookupDescription": "LOADING",                "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1371, "lookupCode": "UNLOAD", "lookupDescription": "UNLOADING",              "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1372, "lookupCode": "ALL",    "lookupDescription": "LOADING AND UNLOADING",  "lookupCategory": "LoadingUnLoading", "image": "" },
  { "lookupID": 1373, "lookupCode": "NONE",   "lookupDescription": "NONE",                   "lookupCategory": "LoadingUnLoading", "image": "" }
]
```

---

## 24. Refresh Token

**POST** `/api/auth/refresh`

**Request:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...<original>",
  "refreshToken": "Qv6EwelG4d8p+dx4OKmun+QqwN+mV37eWeBUf4h2rDfDpZ1noi9NOsnVXZgEUKreDIF7b64iAmla7GXduASEsQ=="
}
```

**Response 200:** ✅ *Tested and confirmed — new tokens issued, old refresh token revoked*
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...<new>",
  "refreshToken": "x8P6Nw/dLfhTrDGkMyxEDqgBmICiyrc1BR8YQIeuAzFSKCVwHyomykJ5O3JxcLZKIv5052ebZrIa8cjzfws1QA==",
  "expiresIn": 3600,
  "userType": "DRIVER",
  "userId": "DR260200001"
}
```

---

## 25. Logout

**POST** `/api/auth/logout`
**Header:** `Authorization: Bearer <accessToken>`

**Response 200:** ✅ *Tested and confirmed*
```json
{ "message": "Logged out successfully" }
```

> **Side effects:** All active refresh tokens revoked. `Operation.DriverActivity` updated — `IsLogIn=false`, `LogoutDate`, `IsOnDuty=false`, `DutyOffDate`, `IsBusy=false`.

---

## Test Summary

| # | Endpoint | Method | Status | Live Tested |
|---|----------|--------|--------|-------------|
| 1 | `/api/auth/driver/login` | POST | ✅ 200 | ✅ |
| 2 | `/api/master/drivers/{driverId}` | GET | ✅ 200 | ✅ |
| 3 | `/api/booking/bookings/nearby` | GET | ✅ 200 | ✅ |
| 4 | `/api/booking/bookings/driver/{driverId}` | GET | ✅ 200 | ✅ |
| 5 | `/api/booking/bookings/confirm` | PUT | ✅ 400* | ✅ |
| 6 | `/api/booking/bookings/cancel-by-driver` | PUT | — | schema only |
| 7 | `/api/booking/bookings/{no}/reach-pickup` | PUT | — | schema only |
| 8 | `/api/booking/bookings/{no}/reach-destination` | PUT | — | schema only |
| 9 | `/api/booking/bookings/{no}/complete` | PUT | — | schema only |
| 10 | `/api/trip/trips/driver/{id}/current` | GET | ✅ 404* | ✅ |
| 11 | `/api/trip/trips/driver/{id}` | GET | ✅ 200 | ✅ |
| 12 | `/api/trip/trips` | POST | — | schema only |
| 13 | `/api/trip/trips/distance` | PATCH | — | schema only |
| 14 | `/api/trip/trips/end` | PUT | — | schema only |
| 15 | `/api/trip/monitors` | POST | ✅ 200 | ✅ |
| 16 | `/api/billing/invoices` | POST | — | schema only |
| 17 | `/api/billing/invoices/pay` | PUT | — | schema only |
| 18 | `/api/billing/drivers/{id}/summary` | GET | ✅ 200 | ✅ |
| 19 | `/api/billing/drivers/{id}/payments` | GET | ✅ 200 | ✅ |
| 20 | `/api/master/lookups/vehicle-types` | GET | ✅ 200 | ✅ |
| 21 | `/api/master/lookups/vehicle-groups` | GET | ✅ 200 | ✅ |
| 22 | `/api/master/lookups/cargo-types` | GET | ✅ 200 | ✅ |
| 23 | `/api/master/lookups/loading-types` | GET | ✅ 200 | ✅ |
| 24 | `/api/auth/refresh` | POST | ✅ 200 | ✅ |
| 25 | `/api/auth/logout` | POST | ✅ 200 | ✅ |

> \* Expected response given no active bookings/trips exist for this driver in the test database.
> "schema only" = endpoint exists and is wired; requires an active booking/trip to produce a success response.

---

*Generated: 2026-04-07 | Driver: DR260200001 | API: PickC Core v1*
