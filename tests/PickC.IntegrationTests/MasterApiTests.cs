using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using PickC.Modules.Master.Application.DTOs;

namespace PickC.IntegrationTests;

public class MasterApiTests : IClassFixture<PickCApiFactory>
{
    private readonly HttpClient _client;

    public MasterApiTests(PickCApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllCustomers_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/master/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SaveCustomer_ThenGetByMobile_ReturnsCustomer()
    {
        var dto = new CustomerSaveDto
        {
            MobileNo = "9876543210",
            Password = "test123",
            Name = "Test Customer",
            EmailID = "test@test.com",
            DeviceID = "device-001"
        };

        var saveResponse = await _client.PostAsJsonAsync("/api/master/customers", dto);
        saveResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getResponse = await _client.GetAsync("/api/master/customers/9876543210");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var customer = await getResponse.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Name.Should().Be("Test Customer");
        customer.MobileNo.Should().Be("9876543210");
    }

    [Fact]
    public async Task DeleteCustomer_ReturnsOk()
    {
        var dto = new CustomerSaveDto
        {
            MobileNo = "1111111111",
            Password = "pass123",
            Name = "Delete Me",
            EmailID = "del@test.com",
            DeviceID = "device-del"
        };

        await _client.PostAsJsonAsync("/api/master/customers", dto);

        var deleteResponse = await _client.DeleteAsync("/api/master/customers/1111111111");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllLookUps_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/master/lookups");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
