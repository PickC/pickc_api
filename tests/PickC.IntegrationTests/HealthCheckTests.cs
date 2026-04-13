using System.Net;
using FluentAssertions;

namespace PickC.IntegrationTests;

public class HealthCheckTests : IClassFixture<PickCApiFactory>
{
    private readonly HttpClient _client;

    public HealthCheckTests(PickCApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task SwaggerEndpoint_ReturnsOk()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("PickC API");
    }
}
