using Microsoft.AspNetCore.Mvc;
using PickC.Modules.Reports.Application.DTOs;
using PickC.Modules.Reports.Application.Services;
using PickC.Modules.Reports.Infrastructure.Email;
using PickC.Modules.Reports.Infrastructure.Generators;
using PickC.SharedKernel.Exceptions;

namespace PickC.Modules.Reports.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportQueryService _queryService;
    private readonly ExcelReportGenerator _excel;
    private readonly PdfReportGenerator _pdf;
    private readonly InvoiceDocumentGenerator _invoiceGenerator;
    private readonly IEmailService _emailService;

    public ReportsController(
        IReportQueryService queryService,
        ExcelReportGenerator excel,
        PdfReportGenerator pdf,
        InvoiceDocumentGenerator invoiceGenerator,
        IEmailService emailService)
    {
        _queryService = queryService;
        _excel = excel;
        _pdf = pdf;
        _invoiceGenerator = invoiceGenerator;
        _emailService = emailService;
    }

    // R1 - Daily Bookings
    [HttpGet("daily-bookings")]
    public async Task<IActionResult> DailyBookings([FromQuery] DateTime? date, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var reportDate = date ?? DateTime.Today;
        var data = await _queryService.GetDailyBookingsAsync(reportDate, ct);

        var excelBytes = () => _excel.Generate(
            "Daily Bookings",
            data.Bookings,
            new[] { "BookingNo", "BookingDate", "CustomerID", "DriverID", "VehicleType", "LocationFrom", "LocationTo", "CargoType", "Status", "IsCancel", "IsComplete" },
            b => new object[] { b.BookingNo, b.BookingDate.ToString("yyyy-MM-dd"), b.CustomerID, b.DriverID, b.VehicleType, b.LocationFrom, b.LocationTo, b.CargoType, b.Status, b.IsCancel, b.IsComplete }
        );
        var pdfBytes = () => _pdf.Generate(
            "Daily Bookings Report",
            $"Date: {reportDate:dd/MM/yyyy} | Total: {data.Total} | Completed: {data.Completed} | Cancelled: {data.Cancelled}",
            data.Bookings,
            new[] { "BookingNo", "Date", "Customer", "Driver", "VehicleType", "From", "To", "CargoType", "Status", "Cancel", "Complete" },
            b => new[] { b.BookingNo, b.BookingDate.ToString("yyyy-MM-dd"), b.CustomerID, b.DriverID, b.VehicleType.ToString(), b.LocationFrom, b.LocationTo, b.CargoType, b.Status.ToString(), b.IsCancel.ToString(), b.IsComplete.ToString() }
        );

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"daily-bookings-{reportDate:yyyy-MM-dd}.xlsx" : $"daily-bookings-{reportDate:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Daily Bookings Report — {reportDate:dd MMM yyyy}", $"<p>Please find attached the Daily Bookings Report for <strong>{reportDate:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Daily Bookings Report emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "excel" => File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-bookings-{reportDate:yyyy-MM-dd}.xlsx"),
            "pdf"   => File(pdfBytes(), "application/pdf", $"daily-bookings-{reportDate:yyyy-MM-dd}.pdf"),
            _       => Ok(data)
        };
    }

    // R2 - Daily Trips
    [HttpGet("daily-trips")]
    public async Task<IActionResult> DailyTrips([FromQuery] DateTime? date, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var reportDate = date ?? DateTime.Today;
        var data = await _queryService.GetDailyTripsAsync(reportDate, ct);

        var excelBytes = () => _excel.Generate(
            "Daily Trips",
            data,
            new[] { "TripID", "TripDate", "CustomerMobile", "DriverID", "VehicleNo", "LocationFrom", "LocationTo", "Distance(km)", "TripMinutes", "StartTime", "EndTime", "BookingNo" },
            t => new object[] { t.TripID, t.TripDate.ToString("yyyy-MM-dd"), t.CustomerMobile, t.DriverID, t.VehicleNo, t.LocationFrom ?? "", t.LocationTo ?? "", t.DistanceKm, t.TripMinutes, t.StartTime.ToString("HH:mm"), t.EndTime?.ToString("HH:mm") ?? "", t.BookingNo ?? "" }
        );
        var pdfBytes = () => _pdf.Generate(
            "Daily Trips Report",
            $"Date: {reportDate:dd/MM/yyyy} | Total Trips: {data.Count}",
            data,
            new[] { "TripID", "Date", "Customer", "Driver", "Vehicle", "From", "To", "Dist(km)", "Minutes", "Start", "End", "Booking" },
            t => new[] { t.TripID, t.TripDate.ToString("yyyy-MM-dd"), t.CustomerMobile, t.DriverID, t.VehicleNo, t.LocationFrom ?? "", t.LocationTo ?? "", t.DistanceKm.ToString("F1"), t.TripMinutes.ToString("F0"), t.StartTime.ToString("HH:mm"), t.EndTime?.ToString("HH:mm") ?? "", t.BookingNo ?? "" }
        );

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"daily-trips-{reportDate:yyyy-MM-dd}.xlsx" : $"daily-trips-{reportDate:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Daily Trips Report — {reportDate:dd MMM yyyy}", $"<p>Please find attached the Daily Trips Report for <strong>{reportDate:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Daily Trips Report emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "excel" => File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-trips-{reportDate:yyyy-MM-dd}.xlsx"),
            "pdf"   => File(pdfBytes(), "application/pdf", $"daily-trips-{reportDate:yyyy-MM-dd}.pdf"),
            _       => Ok(data)
        };
    }

    // R3 - Daily Payments
    [HttpGet("daily-payments")]
    public async Task<IActionResult> DailyPayments([FromQuery] DateTime? date, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var reportDate = date ?? DateTime.Today;
        var data = await _queryService.GetDailyPaymentsAsync(reportDate, ct);

        var excelBytes = () => _excel.Generate(
            "Daily Payments",
            data.Payments,
            new[] { "InvoiceNo", "BookingNo", "TripID", "InvoiceDate", "CustomerMobile", "DriverID", "TripAmount", "TaxAmount", "TipAmount", "TotalAmount", "PaymentType", "PaidAmount", "IsPaid", "PaidDate" },
            p => new object[] { p.InvoiceNo, p.BookingNo, p.TripID, p.InvoiceDate.ToString("yyyy-MM-dd"), p.CustomerMobile, p.DriverID, p.TripAmount, p.TaxAmount, p.TipAmount, p.TotalAmount, p.PaymentType, p.PaidAmount, p.IsPaid, p.PaidDate?.ToString("yyyy-MM-dd") ?? "" },
            new object[] { "TOTAL", "", "", "", "", "", data.TotalTripAmount, data.TotalTaxAmount, data.TotalTipAmount, data.TotalAmount, "", data.TotalPaidAmount, "", "" }
        );
        var pdfBytes = () => _pdf.Generate(
            "Daily Payments Report",
            $"Date: {reportDate:dd/MM/yyyy} | Total: ₹{data.TotalAmount:F2} | Paid: ₹{data.TotalPaidAmount:F2}",
            data.Payments,
            new[] { "InvoiceNo", "BookingNo", "TripID", "Date", "Customer", "Driver", "TripAmt", "Tax", "Tip", "Total", "Type", "Paid", "IsPaid", "PaidDate" },
            p => new[] { p.InvoiceNo, p.BookingNo, p.TripID, p.InvoiceDate.ToString("yyyy-MM-dd"), p.CustomerMobile, p.DriverID, p.TripAmount.ToString("F2"), p.TaxAmount.ToString("F2"), p.TipAmount.ToString("F2"), p.TotalAmount.ToString("F2"), p.PaymentType.ToString(), p.PaidAmount.ToString("F2"), p.IsPaid.ToString(), p.PaidDate?.ToString("yyyy-MM-dd") ?? "" },
            new[] { "TOTAL", "", "", "", "", "", data.TotalTripAmount.ToString("F2"), data.TotalTaxAmount.ToString("F2"), data.TotalTipAmount.ToString("F2"), data.TotalAmount.ToString("F2"), "", data.TotalPaidAmount.ToString("F2"), "", "" }
        );

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"daily-payments-{reportDate:yyyy-MM-dd}.xlsx" : $"daily-payments-{reportDate:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Daily Payments Report — {reportDate:dd MMM yyyy}", $"<p>Please find attached the Daily Payments Report for <strong>{reportDate:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Daily Payments Report emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "excel" => File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-payments-{reportDate:yyyy-MM-dd}.xlsx"),
            "pdf"   => File(pdfBytes(), "application/pdf", $"daily-payments-{reportDate:yyyy-MM-dd}.pdf"),
            _       => Ok(data)
        };
    }

    // R4 - Summary
    [HttpGet("summary")]
    public async Task<IActionResult> Summary([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today;
        var data = await _queryService.GetSummaryAsync(start, end, ct);

        var kpiRows = new[]
        {
            new { KPI = "Total Bookings",    Value = data.TotalBookings.ToString() },
            new { KPI = "Confirmed",         Value = data.ConfirmedBookings.ToString() },
            new { KPI = "Completed",         Value = data.CompletedBookings.ToString() },
            new { KPI = "Cancelled",         Value = data.CancelledBookings.ToString() },
            new { KPI = "Total Trips",       Value = data.TotalTrips.ToString() },
            new { KPI = "Total Distance (km)", Value = data.TotalDistanceKm.ToString("F1") },
            new { KPI = "Total Revenue",     Value = $"₹{data.TotalRevenue:F2}" },
            new { KPI = "Total Tax",         Value = $"₹{data.TotalTaxAmount:F2}" },
            new { KPI = "Total Tips",        Value = $"₹{data.TotalTipAmount:F2}" },
            new { KPI = "Paid Amount",       Value = $"₹{data.TotalPaidAmount:F2}" },
            new { KPI = "Outstanding",       Value = $"₹{data.OutstandingAmount:F2}" },
            new { KPI = "Active Drivers",    Value = data.ActiveDrivers.ToString() },
        };

        var excelBytes = () => _excel.Generate("Summary", kpiRows, new[] { "KPI", "Value" }, r => new object[] { r.KPI, r.Value });
        var pdfBytes   = () => _pdf.Generate("Summary Report", $"Period: {start:dd/MM/yyyy} to {end:dd/MM/yyyy}", kpiRows, new[] { "KPI", "Value" }, r => new[] { r.KPI, r.Value });

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"summary-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx" : $"summary-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Summary Report — {start:dd MMM yyyy} to {end:dd MMM yyyy}", $"<p>Please find attached the Summary Report for the period <strong>{start:dd MMM yyyy}</strong> to <strong>{end:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Summary Report emailed to {email}" });
        }

        if (format.ToLower() == "excel")
            return File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"summary-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx");

        if (format.ToLower() == "pdf")
            return File(pdfBytes(), "application/pdf", $"summary-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf");

        return Ok(data);
    }

    // R5 - Driver Earnings
    [HttpGet("driver-earnings")]
    public async Task<IActionResult> DriverEarnings([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? driverId, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today;
        var data = await _queryService.GetDriverEarningsAsync(start, end, driverId, ct);

        var excelBytes = () => _excel.Generate("Driver Earnings", data,
            new[] { "DriverID", "DriverName", "MobileNo", "TotalTrips", "Distance(km)", "TripAmount", "TaxAmount", "TipAmount", "TotalEarnings", "PaidAmount", "Outstanding" },
            d => new object[] { d.DriverID, d.DriverName, d.MobileNo, d.TotalTrips, d.TotalDistanceKm, d.TripAmount, d.TaxAmount, d.TipAmount, d.TotalEarnings, d.PaidAmount, d.Outstanding });
        var pdfBytes = () => _pdf.Generate("Driver Earnings Report", $"Period: {start:dd/MM/yyyy} to {end:dd/MM/yyyy}",
            data,
            new[] { "DriverID", "Name", "Mobile", "Trips", "Dist(km)", "TripAmt", "Tax", "Tip", "Total", "Paid", "Outstanding" },
            d => new[] { d.DriverID, d.DriverName, d.MobileNo, d.TotalTrips.ToString(), d.TotalDistanceKm.ToString("F1"), d.TripAmount.ToString("F2"), d.TaxAmount.ToString("F2"), d.TipAmount.ToString("F2"), d.TotalEarnings.ToString("F2"), d.PaidAmount.ToString("F2"), d.Outstanding.ToString("F2") });

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"driver-earnings-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx" : $"driver-earnings-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Driver Earnings Report — {start:dd MMM yyyy} to {end:dd MMM yyyy}", $"<p>Please find attached the Driver Earnings Report for the period <strong>{start:dd MMM yyyy}</strong> to <strong>{end:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Driver Earnings Report emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "excel" => File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"driver-earnings-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx"),
            "pdf"   => File(pdfBytes(), "application/pdf", $"driver-earnings-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf"),
            _       => Ok(data)
        };
    }

    // R6 - Driver Tips
    [HttpGet("driver-tips")]
    public async Task<IActionResult> DriverTips([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? driverId, [FromQuery] string format = "json", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var start = startDate ?? DateTime.Today.AddDays(-30);
        var end = endDate ?? DateTime.Today;
        var data = await _queryService.GetDriverTipsAsync(start, end, driverId, ct);

        var excelBytes = () => _excel.Generate("Driver Tips", data.Tips,
            new[] { "Date", "InvoiceNo", "TripID", "BookingNo", "CustomerMobile", "DriverID", "DriverName", "TipAmount" },
            t => new object[] { t.Date.ToString("yyyy-MM-dd"), t.InvoiceNo, t.TripID, t.BookingNo, t.CustomerMobile, t.DriverID, t.DriverName, t.TipAmount },
            new object[] { "TOTAL", "", "", "", "", "", "", data.TotalTips });
        var pdfBytes = () => _pdf.Generate("Driver Tip Report", $"Period: {start:dd/MM/yyyy} to {end:dd/MM/yyyy} | Total Tips: ₹{data.TotalTips:F2}",
            data.Tips,
            new[] { "Date", "InvoiceNo", "TripID", "Booking", "Customer", "DriverID", "DriverName", "Tip" },
            t => new[] { t.Date.ToString("yyyy-MM-dd"), t.InvoiceNo, t.TripID, t.BookingNo, t.CustomerMobile, t.DriverID, t.DriverName, t.TipAmount.ToString("F2") },
            new[] { "TOTAL", "", "", "", "", "", "", data.TotalTips.ToString("F2") });

        if (!string.IsNullOrEmpty(email))
        {
            var isExcel = format.ToLower() == "excel";
            var bytes = isExcel ? excelBytes() : pdfBytes();
            var fileName = isExcel ? $"driver-tips-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx" : $"driver-tips-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf";
            var mime = isExcel ? "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            await _emailService.SendWithAttachmentAsync(email, $"PickC Driver Tips Report — {start:dd MMM yyyy} to {end:dd MMM yyyy}", $"<p>Please find attached the Driver Tips Report for the period <strong>{start:dd MMM yyyy}</strong> to <strong>{end:dd MMM yyyy}</strong>.</p>", bytes, fileName, mime, ct);
            return Ok(new { message = $"Driver Tips Report emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "excel" => File(excelBytes(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"driver-tips-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.xlsx"),
            "pdf"   => File(pdfBytes(), "application/pdf", $"driver-tips-{start:yyyy-MM-dd}-to-{end:yyyy-MM-dd}.pdf"),
            _       => Ok(data)
        };
    }

    // R7 - Invoice Download / Email
    [HttpGet("invoice/{invoiceNo}/{tripId}")]
    public async Task<IActionResult> GetInvoice(string invoiceNo, string tripId, [FromQuery] string format = "pdf", [FromQuery] string? email = null, CancellationToken ct = default)
    {
        var invoice = await _queryService.GetInvoiceAsync(invoiceNo, tripId, ct);

        if (!string.IsNullOrEmpty(email))
        {
            var pdfBytes = _invoiceGenerator.GeneratePdf(invoice);
            var html = _invoiceGenerator.GenerateHtml(invoice);
            await _emailService.SendWithAttachmentAsync(email, $"PickC Invoice #{invoiceNo}", html, pdfBytes, $"invoice-{invoiceNo}.pdf", "application/pdf", ct);
            return Ok(new { message = $"Invoice {invoiceNo} emailed to {email}" });
        }

        return format.ToLower() switch
        {
            "html" => Content(_invoiceGenerator.GenerateHtml(invoice), "text/html"),
            _      => File(_invoiceGenerator.GeneratePdf(invoice), "application/pdf", $"invoice-{invoiceNo}.pdf")
        };
    }

    // R8 - Email Invoice
    [HttpPost("invoice/{invoiceNo}/{tripId}/email")]
    public async Task<IActionResult> EmailInvoice(string invoiceNo, string tripId, [FromQuery] string? testEmail = null, CancellationToken ct = default)
    {
        var invoice = await _queryService.GetInvoiceAsync(invoiceNo, tripId, ct);

        var recipientEmail = testEmail ?? invoice.CustomerEmail;
        if (string.IsNullOrEmpty(recipientEmail))
            return BadRequest(new { message = "Customer has no email address on file." });

        var html = _invoiceGenerator.GenerateHtml(invoice);
        var pdfBytes = _invoiceGenerator.GeneratePdf(invoice);
        await _emailService.SendWithAttachmentAsync(
            invoice.CustomerEmail,
            $"Your PickC Invoice #{invoiceNo}",
            html,
            pdfBytes,
            $"invoice-{invoiceNo}.pdf",
            "application/pdf",
            ct);
        await _queryService.MarkInvoiceMailSentAsync(invoiceNo, tripId, ct);

        return Ok(new { message = $"Invoice emailed to {invoice.CustomerEmail}" });
    }
}
