using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PickC.Modules.Booking.Application.DTOs;

namespace PickC.IntegrationTests;

public class BookingApiTests : IClassFixture<PickCApiFactory>
{
    private readonly HttpClient _client;

    public BookingApiTests(PickCApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBookings_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/booking/bookings");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveBooking_ThenGetById_ReturnsBooking()
    {
        var dto = new BookingSaveDto
        {
            BookingNo = "BK-TEST-001",
            CustomerID = "CUST001",
            RequiredDate = DateTime.UtcNow.AddDays(1),
            LocationFrom = "123 Main St, Delhi",
            LocationTo = "456 Park Ave, Mumbai",
            CargoDescription = "Electronics",
            VehicleType = 1,
            CargoType = "Fragile",
            PayLoad = "500kg",
            Latitude = 28.6139m,
            Longitude = 77.2090m,
            ToLatitude = 19.0760m,
            ToLongitude = 72.8777m,
            ReceiverMobileNo = "9998887776",
            Status = 0
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/booking/bookings", dto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/booking/bookings/BK-TEST-001");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var booking = await getResponse.Content.ReadFromJsonAsync<BookingDto>();
        booking.Should().NotBeNull();
        booking!.BookingNo.Should().Be("BK-TEST-001");
        booking.CustomerID.Should().Be("CUST001");
        booking.LocationFrom.Should().Be("123 Main St, Delhi");
    }

    [Fact]
    public async Task SaveBooking_ThenRetrieveByCustomer_ReturnsBooking()
    {
        var saveDto = new BookingSaveDto
        {
            BookingNo = "BK-TEST-002",
            CustomerID = "CUST002",
            RequiredDate = DateTime.UtcNow.AddDays(1),
            LocationFrom = "Origin",
            LocationTo = "Destination",
            CargoDescription = "Goods",
            VehicleType = 1,
            CargoType = "General",
            PayLoad = "200kg",
            Latitude = 28.6m,
            Longitude = 77.2m,
            ToLatitude = 19.0m,
            ToLongitude = 72.8m,
            ReceiverMobileNo = "9998887775",
            Status = 0
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/booking/bookings", saveDto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify by getting customer bookings
        var getResponse = await _client.GetAsync("/api/booking/bookings/customer/CUST002");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SearchBookingsToday_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/booking/search/today");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetNearBookings_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/booking/bookings/nearby?lat=28.6&lng=77.2&range=50");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
