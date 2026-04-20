# Reports Module

This document describes the Reports module for the PickC API, covering the database schema change, all report endpoints, email configuration, invoice layout, and third-party libraries used.

---

## Table of Contents

1. [DB Schema Change](#1-db-schema-change)
2. [Report Endpoints](#2-report-endpoints)
   - [R1 — Daily Bookings Report](#r1--daily-bookings-report)
   - [R2 — Daily Trip Report](#r2--daily-trip-report)
   - [R3 — Daily Payment Report](#r3--daily-payment-report)
   - [R4 — Summary Report](#r4--summary-report)
   - [R5 — Driver Earnings Report](#r5--driver-earnings-report)
   - [R6 — Driver Tip Report](#r6--driver-tip-report)
   - [R7 — Invoice Download](#r7--invoice-download)
   - [R8 — Email Invoice to Customer](#r8--email-invoice-to-customer)
3. [Email Setup](#3-email-setup)
4. [Invoice Layout](#4-invoice-layout)
5. [Libraries Used](#5-libraries-used)

---

## 1. DB Schema Change

A `TipAmount` column was added to the `Operation.Invoice` table to support driver tip tracking in invoices and reports.

| Column      | Type              | Constraint            |
|-------------|-------------------|-----------------------|
| TipAmount   | numeric(18,2)     | NOT NULL DEFAULT 0    |

**Migration SQL:**

```sql
ALTER TABLE Operation.Invoice ADD TipAmount numeric(18,2) NOT NULL DEFAULT 0;
```

---

## 2. Report Endpoints

### Base URL

```
GET /api/reports/{report-type}
```

### Format Parameter

All report endpoints accept an optional `format` query parameter:

| Value   | Output                        |
|---------|-------------------------------|
| `json`  | JSON response (default)       |
| `excel` | Excel file (.xlsx) download   |
| `pdf`   | PDF file download             |

---

### R1 — Daily Bookings Report

```
GET /api/reports/daily-bookings?date=YYYY-MM-DD&format=json|excel|pdf
```

Returns all bookings for a given date.

**Source table:** `Operation.Booking`

**Columns returned:**

| Column       | Description                        |
|--------------|------------------------------------|
| BookingNo    | Booking reference number           |
| BookingDate  | Date of the booking                |
| CustomerID   | Customer identifier                |
| DriverID     | Assigned driver identifier         |
| VehicleType  | Type of vehicle requested          |
| LocationFrom | Pickup location                    |
| LocationTo   | Drop-off location                  |
| CargoType    | Type of cargo/passengers           |
| Status       | Current booking status             |
| IsCancel     | Whether the booking was cancelled  |
| IsComplete   | Whether the booking was completed  |

**Response summary (JSON):**

```json
{
  "date": "2026-04-02",
  "summary": {
    "Total": 42,
    "Confirmed": 10,
    "Completed": 28,
    "Cancelled": 4
  },
  "data": [ ... ]
}
```

---

### R2 — Daily Trip Report

```
GET /api/reports/daily-trips?date=YYYY-MM-DD&format=json|excel|pdf
```

Returns all trips completed or recorded on a given date.

**Source table:** `Operation.Trip`

**Columns returned:**

| Column          | Description                           |
|-----------------|---------------------------------------|
| TripID          | Unique trip identifier                |
| TripDate        | Date of the trip                      |
| CustomerMobile  | Customer contact number               |
| DriverID        | Driver who completed the trip         |
| VehicleNo       | Vehicle registration number           |
| LocationFrom    | Trip start location                   |
| LocationTo      | Trip end location                     |
| Distance (km)   | Distance covered in kilometres        |
| TripMinutes     | Duration of the trip in minutes       |
| StartTime       | Trip start timestamp                  |
| EndTime         | Trip end timestamp                    |
| BookingNo       | Associated booking reference          |

---

### R3 — Daily Payment Report

```
GET /api/reports/daily-payments?date=YYYY-MM-DD&format=json|excel|pdf
```

Returns all invoice/payment records for a given date, including tip amounts.

**Source:** `Operation.Invoice` JOIN `Operation.Trip`

**Columns returned:**

| Column         | Description                                    |
|----------------|------------------------------------------------|
| InvoiceNo      | Invoice reference number                       |
| BookingNo      | Associated booking reference                   |
| TripID         | Associated trip identifier                     |
| InvoiceDate    | Date the invoice was generated                 |
| CustomerMobile | Customer contact number                        |
| DriverID       | Driver identifier                              |
| TripAmount     | Base fare for the trip                         |
| TaxAmount      | Tax (GST) applied                              |
| TipAmount      | Tip amount given to the driver                 |
| TotalAmount    | Total invoiced amount                          |
| PaymentType    | Payment method (Cash, Card, Wallet, etc.)      |
| PaidAmount     | Amount actually paid                           |
| IsPaid         | Whether the invoice has been settled           |
| PaidDate       | Date payment was received                      |

**Footer totals (JSON / Excel / PDF):**

The response includes column-level totals for:
- `TripAmount`
- `TaxAmount`
- `TipAmount`
- `TotalAmount`
- `PaidAmount`

---

### R4 — Summary Report

```
GET /api/reports/summary?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD&format=json|excel|pdf
```

Returns aggregated KPIs for a date range.

**Key Performance Indicators:**

| KPI                  | Description                                      |
|----------------------|--------------------------------------------------|
| Total Bookings       | Number of bookings in the period                 |
| Total Trips          | Number of trips completed in the period          |
| Total Revenue        | Sum of TotalAmount for all invoices              |
| Payment Split        | Revenue breakdown by PaymentType                 |
| Top 5 Drivers        | Drivers ranked by earnings/trips in the period   |

---

### R5 — Driver Earnings Report

```
GET /api/reports/driver-earnings?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD&driverId=&format=json|excel|pdf
```

Returns a per-driver breakdown of earnings for a date range. The `driverId` parameter is optional; if omitted, results are returned for all drivers.

**Data includes:**
- Number of trips completed
- Base trip fare total
- Tax total
- Tip total
- Overall earnings total

---

### R6 — Driver Tip Report

```
GET /api/reports/driver-tips?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD&driverId=&format=json|excel|pdf
```

Returns a detailed listing of tips received per driver for a date range. The `driverId` parameter is optional; if omitted, results are returned for all drivers.

**Data includes:**
- Per-trip tip detail (InvoiceNo, TripID, TripDate, TipAmount)
- Aggregated tip total per driver

---

### R7 — Invoice Download

```
GET /api/reports/invoice/{invoiceNo}/{tripId}?format=html|pdf
```

Returns an Uber/Ola style formatted invoice for a specific invoice and trip.

**Path parameters:**

| Parameter  | Description            |
|------------|------------------------|
| invoiceNo  | Invoice reference number |
| tripId     | Associated trip ID     |

**Query parameter:**

| Value  | Response Content-Type  | Description             |
|--------|------------------------|-------------------------|
| `html` | `text/html`            | Inline HTML invoice     |
| `pdf`  | `application/pdf`      | Downloadable PDF        |

The invoice includes a full fare breakdown table. See [Invoice Layout](#4-invoice-layout) for details.

---

### R8 — Email Invoice to Customer

```
POST /api/reports/invoice/{invoiceNo}/{tripId}/email
```

Sends the HTML invoice to the customer's registered email address via SMTP. On success, sets `IsMailSent = true` on the invoice record.

**Path parameters:**

| Parameter  | Description              |
|------------|--------------------------|
| invoiceNo  | Invoice reference number |
| tripId     | Associated trip ID       |

**Success response:**

```json
{
  "message": "Invoice emailed to customer@example.com"
}
```

**Behaviour:**
- Fetches customer email from the booking/customer record linked to the invoice.
- Renders the same HTML invoice template used by R7.
- Sends via SMTP using MailKit (see [Email Setup](#3-email-setup)).
- Updates `Operation.Invoice.IsMailSent = true` upon successful delivery.

---

## 3. Email Setup

Configure SMTP credentials in `appsettings.json`:

```json
"SmtpSettings": {
  "Host": "smtp.yourdomain.com",
  "Port": 587,
  "UserName": "your-username",
  "Password": "your-password",
  "From": "noreply@pickc.com",
  "FromName": "PickC"
}
```

**Notes:**
- The application uses **MailKit 4.x** for SMTP communication.
- Port `587` with **STARTTLS** is the supported configuration.
- Replace `Host`, `UserName`, and `Password` with your actual SMTP provider credentials before deployment.
- The `From` address is shown as the sender in the customer's inbox.

---

## 4. Invoice Layout

The invoice follows an Uber/Ola style single-page layout suitable for both HTML rendering and PDF export.

```
+-------------------------------------------------------+
|  [PickC Logo Placeholder]   PickC         TAX INVOICE |
+-------------------------------------------------------+
|  Invoice No : INV-00123                               |
|  Date       : 02 Apr 2026                             |
|  Booking No : BK-00456                                |
+-------------------------------------------------------+
|  CUSTOMER                    DRIVER                   |
|  Name / Mobile               Driver ID / Vehicle No   |
+-------------------------------------------------------+
|  TRIP DETAILS                                         |
|  From  : [Pickup Location]                            |
|  To    : [Drop-off Location]                          |
|  Distance : 12.4 km    Duration : 23 min              |
+-------------------------------------------------------+
|  FARE BREAKDOWN                                       |
|  +-----------------------+-----------+               |
|  | Description           | Amount    |               |
|  +-----------------------+-----------+               |
|  | Base Trip Fare        |  $  xx.xx |               |
|  | Tax (GST)             |  $  xx.xx |               |
|  | Driver Tip            |  $  xx.xx |               |
|  +-----------------------+-----------+               |
|  | TOTAL                 |  $  xx.xx |               |
|  +-----------------------+-----------+               |
+-------------------------------------------------------+
|  Payment Status:  [ PAID ]  /  [ OUTSTANDING ]        |
|  Payment Type  :  Cash / Card / Wallet                 |
+-------------------------------------------------------+
|          Thank you for using PickC                    |
+-------------------------------------------------------+
```

**Section details:**

| Section          | Content                                                                 |
|------------------|-------------------------------------------------------------------------|
| Header           | PickC logo placeholder, company name, "TAX INVOICE" label              |
| Invoice Metadata | InvoiceNo, Invoice Date, BookingNo                                      |
| Customer         | Customer name and mobile number                                         |
| Driver           | DriverID and vehicle registration number                                |
| Trip Details     | LocationFrom, LocationTo, Distance (km), Duration (minutes)             |
| Fare Breakdown   | Base Trip Fare, Tax (GST), Driver Tip, Total — presented as a table     |
| Payment Status   | Displays "PAID" or "OUTSTANDING" based on `IsPaid`; shows payment type  |
| Footer           | "Thank you for using PickC"                                             |

---

## 5. Libraries Used

| Library          | Version       | Purpose                          | License            |
|------------------|---------------|----------------------------------|--------------------|
| QuestPDF         | 2024.x        | PDF generation for reports and invoices | Community License |
| ClosedXML        | 0.102.x       | Excel (.xlsx) export             | MIT                |
| MailKit          | 4.x           | SMTP email delivery              | MIT                |

**Notes:**

- **QuestPDF** (Community License) is free for open-source and small commercial projects. Review the [QuestPDF licensing page](https://www.questpdf.com/license/) if the project scales to ensure compliance.
- **ClosedXML** produces `.xlsx` files compatible with Microsoft Excel and other spreadsheet applications.
- **MailKit** is the recommended .NET SMTP client; it replaces the legacy `System.Net.Mail.SmtpClient`.
