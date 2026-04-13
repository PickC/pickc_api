using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PickC.Modules.Trip.Application.DTOs;

namespace PickC.IntegrationTests;

public class TripApiTests : IClassFixture<PickCApiFactory>
{
    private readonly HttpClient _client;

    public TripApiTests(PickCApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllTrips_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/trip/trips");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveTrip_ThenGetById_ReturnsTrip()
    {
        var saveDto = new TripSaveDto
        {
            TripID = "TRP-TEST-001",
            CustomerMobile = "9876543210",
            DriverID = "DRV001",
            VehicleNo = "DL01AB1234",
            VehicleType = 1,
            VehicleGroup = 1,
            LocationFrom = "Delhi",
            LocationTo = "Mumbai",
            Distance = 1400.50m,
            TotalWeight = "500kg",
            CargoDescription = "Electronics",
            Latitude = 28.6139m,
            Longitude = 77.2090m,
            BookingNo = "BK-001"
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/trip/trips", saveDto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Get trip
        var getResponse = await _client.GetAsync("/api/trip/trips/TRP-TEST-001");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var trip = await getResponse.Content.ReadFromJsonAsync<TripDto>();
        trip.Should().NotBeNull();
        trip!.TripID.Should().Be("TRP-TEST-001");
        trip.DriverID.Should().Be("DRV001");
        trip.EndTime.Should().BeNull();
    }

    [Fact]
    public async Task GetCurrentTripByDriver_WhenNoTrip_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/trip/trips/driver/NONEXISTENT/current");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SaveTripMonitor_ReturnsOk()
    {
        var dto = new TripMonitorSaveDto
        {
            DriverID = "DRV001",
            TripID = "TRP-MON-001",
            VehicleNo = "DL01AB1234",
            Latitude = 28.6m,
            Longitude = 77.2m,
            TripType = 1
        };

        var response = await _client.PostAsJsonAsync("/api/trip/monitors", dto);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllTripMonitors_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/trip/monitors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
