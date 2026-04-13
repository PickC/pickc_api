using System.Reflection;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PickC.Modules.Reports.Application.DTOs;

namespace PickC.Modules.Reports.Infrastructure.Generators;

public class InvoiceDocumentGenerator
{
    private readonly byte[] _logoBytes;
    private readonly string _logoBase64;

    public InvoiceDocumentGenerator()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames()
            .FirstOrDefault(n => n.EndsWith("pickc_logo.png"));

        if (resourceName != null)
        {
            using var stream = assembly.GetManifestResourceStream(resourceName)!;
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            _logoBytes = ms.ToArray();
            _logoBase64 = Convert.ToBase64String(_logoBytes);
        }
        else
        {
            _logoBytes = Array.Empty<byte>();
            _logoBase64 = string.Empty;
        }
    }

    public byte[] GeneratePdf(InvoiceReportDto invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content().Column(col =>
                {
                    // Header
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            if (_logoBytes.Length > 0)
                                c.Item().Width(80).Image(_logoBytes);
                            else
                                c.Item().Text("PickC").Bold().FontSize(28).FontColor(Colors.Blue.Darken2);
                            c.Item().Text("Logistics & Ride Service").FontSize(10).FontColor(Colors.Grey.Darken1);
                        });
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("TAX INVOICE").Bold().FontSize(18).FontColor(Colors.Blue.Darken2);
                            c.Item().Text($"Invoice #: {invoice.InvoiceNo}").FontSize(10);
                            c.Item().Text($"Date: {invoice.InvoiceDate:dd/MM/yyyy}").FontSize(10);
                            c.Item().Text($"Booking: {invoice.BookingNo}").FontSize(10);
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                    // Customer + Driver info
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("CUSTOMER").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            c.Item().Text(invoice.CustomerName).FontSize(11);
                            c.Item().Text(invoice.CustomerMobile).FontSize(10).FontColor(Colors.Grey.Darken1);
                            c.Item().Text(invoice.CustomerEmail).FontSize(10).FontColor(Colors.Grey.Darken1);
                        });
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("DRIVER").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            c.Item().Text(invoice.DriverName).FontSize(11);
                            c.Item().Text($"ID: {invoice.DriverID}").FontSize(10).FontColor(Colors.Grey.Darken1);
                            c.Item().Text($"Vehicle: {invoice.VehicleNo}").FontSize(10).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    // Trip details
                    col.Item().Column(c =>
                    {
                        c.Item().Text("TRIP DETAILS").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().PaddingTop(4).Row(r =>
                        {
                            r.RelativeItem().Text($"From: {invoice.LocationFrom ?? "N/A"}");
                            r.RelativeItem().Text($"To: {invoice.LocationTo ?? "N/A"}");
                        });
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"Distance: {invoice.DistanceKm:F1} km");
                            r.RelativeItem().Text($"Duration: {invoice.TripMinutes:F0} min");
                        });
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"Start: {invoice.StartTime:dd/MM/yyyy HH:mm}");
                            r.RelativeItem().Text($"End: {(invoice.EndTime.HasValue ? invoice.EndTime.Value.ToString("dd/MM/yyyy HH:mm") : "N/A")}");
                        });
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    // Fare breakdown table
                    col.Item().Text("FARE BREAKDOWN").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                    col.Item().PaddingTop(6).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(3);
                            cols.RelativeColumn(1);
                        });

                        // Table header
                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Blue.Darken2).Padding(6).Text("Item").Bold().FontColor(Colors.White);
                            header.Cell().Background(Colors.Blue.Darken2).Padding(6).AlignRight().Text("Amount").Bold().FontColor(Colors.White);
                        });

                        // Rows
                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text("Base Trip Fare");
                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"₹ {invoice.TripAmount:F2}");

                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text("Tax (GST)");
                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"₹ {invoice.TaxAmount:F2}");

                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Text("Driver Tip");
                        table.Cell().Padding(6).BorderBottom(1).BorderColor(Colors.Grey.Lighten2).AlignRight().Text($"₹ {invoice.TipAmount:F2}");

                        table.Cell().Background(Colors.Blue.Lighten4).Padding(6).Text("Total").Bold();
                        table.Cell().Background(Colors.Blue.Lighten4).Padding(6).AlignRight().Text($"₹ {invoice.TotalAmount:F2}").Bold();
                    });

                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                    // Payment status
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("PAYMENT").Bold().FontSize(10).FontColor(Colors.Grey.Darken2);
                            var paymentTypeName = invoice.PaymentType switch { 1 => "Cash", 2 => "Credit", 3 => "Online", _ => "N/A" };
                            c.Item().Text($"Method: {paymentTypeName}").FontSize(10);
                            c.Item().Text($"Paid: ₹ {invoice.PaidAmount:F2}").FontSize(10);
                        });
                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            var (statusText, statusColor) = invoice.IsPaid
                                ? ("PAID", Colors.Green.Darken2)
                                : ("OUTSTANDING", Colors.Red.Darken2);
                            c.Item().Background(invoice.IsPaid ? Colors.Green.Lighten4 : Colors.Red.Lighten4)
                                .Padding(8).Text(statusText).Bold().FontSize(16).FontColor(statusColor);
                            if (invoice.PaidDate.HasValue)
                                c.Item().Text($"Paid on: {invoice.PaidDate.Value:dd/MM/yyyy}").FontSize(9).FontColor(Colors.Grey.Darken1);
                        });
                    });

                    col.Item().PaddingVertical(16).LineHorizontal(2).LineColor(Colors.Blue.Darken2);

                    // Footer
                    col.Item().AlignCenter().Text("Thank you for using PickC").Italic().FontSize(12).FontColor(Colors.Blue.Darken2);
                });
            });
        });

        return document.GeneratePdf();
    }

    public string GenerateHtml(InvoiceReportDto invoice)
    {
        var invoiceNo = System.Net.WebUtility.HtmlEncode(invoice.InvoiceNo);
        var bookingNo = System.Net.WebUtility.HtmlEncode(invoice.BookingNo);
        var customerName = System.Net.WebUtility.HtmlEncode(invoice.CustomerName);
        var customerMobile = System.Net.WebUtility.HtmlEncode(invoice.CustomerMobile);
        var customerEmail = System.Net.WebUtility.HtmlEncode(invoice.CustomerEmail);
        var driverName = System.Net.WebUtility.HtmlEncode(invoice.DriverName);
        var driverId = System.Net.WebUtility.HtmlEncode(invoice.DriverID);
        var vehicleNo = System.Net.WebUtility.HtmlEncode(invoice.VehicleNo);
        var locationFrom = System.Net.WebUtility.HtmlEncode(invoice.LocationFrom ?? "N/A");
        var locationTo = System.Net.WebUtility.HtmlEncode(invoice.LocationTo ?? "N/A");

        var paymentTypeName = invoice.PaymentType switch { 1 => "Cash", 2 => "Credit", 3 => "Online", _ => "N/A" };
        var statusText = invoice.IsPaid ? "PAID" : "OUTSTANDING";
        var statusColor = invoice.IsPaid ? "#2e7d32" : "#c62828";
        var statusBg = invoice.IsPaid ? "#e8f5e9" : "#ffebee";
        var endTimeStr = invoice.EndTime.HasValue
            ? invoice.EndTime.Value.ToString("dd/MM/yyyy HH:mm")
            : "N/A";
        var paidDateHtml = invoice.PaidDate.HasValue
            ? $"<div>Paid on: {invoice.PaidDate.Value:dd/MM/yyyy}</div>"
            : "";
        var logoHtml = _logoBase64.Length > 0
            ? $"<img src=\"data:image/png;base64,{_logoBase64}\" alt=\"PickC\" style=\"height:50px;\">"
            : "<span style=\"color:#1565c0;font-size:28px;font-weight:bold;\">PickC</span>";

        return $$"""
        <!DOCTYPE html>
        <html>
        <head>
            <meta charset="utf-8">
            <title>Invoice {{invoiceNo}}</title>
            <style>
                body { font-family: Arial, sans-serif; max-width: 700px; margin: 0 auto; padding: 20px; color: #333; }
                .header { display: flex; justify-content: space-between; align-items: flex-start; border-bottom: 3px solid #1565c0; padding-bottom: 16px; margin-bottom: 16px; }
                .company { color: #1565c0; font-size: 28px; font-weight: bold; }
                .invoice-meta { text-align: right; }
                .invoice-title { color: #1565c0; font-size: 20px; font-weight: bold; }
                .section { display: flex; gap: 20px; margin-bottom: 16px; }
                .section-block { flex: 1; }
                .label { font-weight: bold; color: #666; font-size: 11px; text-transform: uppercase; margin-bottom: 4px; }
                .divider { border: none; border-top: 1px solid #ddd; margin: 16px 0; }
                table { width: 100%; border-collapse: collapse; }
                th { background: #1565c0; color: white; padding: 8px; text-align: left; }
                th:last-child, td:last-child { text-align: right; }
                td { padding: 8px; border-bottom: 1px solid #eee; }
                .total-row td { background: #bbdefb; font-weight: bold; }
                .status { display: inline-block; padding: 8px 16px; border-radius: 4px; font-weight: bold; font-size: 18px; background: {{statusBg}}; color: {{statusColor}}; }
                .footer { text-align: center; color: #1565c0; font-style: italic; margin-top: 24px; padding-top: 16px; border-top: 2px solid #1565c0; }
            </style>
        </head>
        <body>
            <div class="header">
                <div>
                    {{logoHtml}}
                    <div style="color:#666;margin-top:4px">Logistics &amp; Ride Service</div>
                </div>
                <div class="invoice-meta">
                    <div class="invoice-title">TAX INVOICE</div>
                    <div>Invoice #: {{invoiceNo}}</div>
                    <div>Date: {{invoice.InvoiceDate:dd/MM/yyyy}}</div>
                    <div>Booking: {{bookingNo}}</div>
                </div>
            </div>

            <div class="section">
                <div class="section-block">
                    <div class="label">Customer</div>
                    <div><strong>{{customerName}}</strong></div>
                    <div>{{customerMobile}}</div>
                    <div>{{customerEmail}}</div>
                </div>
                <div class="section-block">
                    <div class="label">Driver</div>
                    <div><strong>{{driverName}}</strong></div>
                    <div>ID: {{driverId}}</div>
                    <div>Vehicle: {{vehicleNo}}</div>
                </div>
            </div>

            <hr class="divider">

            <div class="label">Trip Details</div>
            <div class="section" style="margin-top:8px">
                <div class="section-block">
                    <div>From: {{locationFrom}}</div>
                    <div>To: {{locationTo}}</div>
                </div>
                <div class="section-block">
                    <div>Distance: {{invoice.DistanceKm:F1}} km</div>
                    <div>Duration: {{invoice.TripMinutes:F0}} min</div>
                    <div>Start: {{invoice.StartTime:dd/MM/yyyy HH:mm}}</div>
                    <div>End: {{endTimeStr}}</div>
                </div>
            </div>

            <hr class="divider">

            <div class="label" style="margin-bottom:8px">Fare Breakdown</div>
            <table>
                <thead><tr><th>Item</th><th>Amount</th></tr></thead>
                <tbody>
                    <tr><td>Base Trip Fare</td><td>&#8377; {{invoice.TripAmount:F2}}</td></tr>
                    <tr><td>Tax (GST)</td><td>&#8377; {{invoice.TaxAmount:F2}}</td></tr>
                    <tr><td>Driver Tip</td><td>&#8377; {{invoice.TipAmount:F2}}</td></tr>
                    <tr class="total-row"><td><strong>Total</strong></td><td><strong>&#8377; {{invoice.TotalAmount:F2}}</strong></td></tr>
                </tbody>
            </table>

            <hr class="divider">

            <div class="section" style="align-items:center">
                <div>
                    <div class="label">Payment</div>
                    <div>Method: {{paymentTypeName}}</div>
                    <div>Paid: &#8377; {{invoice.PaidAmount:F2}}</div>
                    {{paidDateHtml}}
                </div>
                <div style="text-align:right">
                    <span class="status">{{statusText}}</span>
                </div>
            </div>

            <div class="footer">Thank you for using PickC</div>
        </body>
        </html>
        """;
    }
}
