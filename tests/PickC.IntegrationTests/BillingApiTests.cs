using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PickC.Modules.Billing.Application.DTOs;

namespace PickC.IntegrationTests;

public class BillingApiTests : IClassFixture<PickCApiFactory>
{
    private readonly HttpClient _client;

    public BillingApiTests(PickCApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllInvoices_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/billing/invoices");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveInvoice_ThenGetByKey_ReturnsInvoice()
    {
        var dto = new InvoiceSaveDto
        {
            InvoiceNo = "INV-TEST-001",
            TripID = "TRP-001",
            TripAmount = 1500.00m,
            TaxAmount = 270.00m,
            TotalAmount = 1770.00m,
            PaymentType = 1,
            BookingNo = "BK-001"
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/billing/invoices", dto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/billing/invoices/INV-TEST-001/TRP-001");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.InvoiceNo.Should().Be("INV-TEST-001");
        invoice.TripAmount.Should().Be(1500.00m);
        invoice.TotalAmount.Should().Be(1770.00m);
    }

    [Fact]
    public async Task SaveInvoice_ThenGetByBookingNo_ReturnsInvoice()
    {
        var saveDto = new InvoiceSaveDto
        {
            InvoiceNo = "INV-TEST-002",
            TripID = "TRP-002",
            TripAmount = 2000.00m,
            TaxAmount = 360.00m,
            TotalAmount = 2360.00m,
            PaymentType = 1,
            BookingNo = "BK-002"
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/billing/invoices", saveDto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/billing/invoices/booking/BK-002");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetInvoiceByBookingNo_WhenNotFound_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/billing/invoices/booking/NONEXISTENT");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
