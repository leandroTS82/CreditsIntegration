using System.Net;
using Credits.Api.IntegrationTests.Fixtures;

namespace Credits.Api.IntegrationTests.Tests;

public sealed class HealthCheckIntegrationTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GET_Self_Returns200()
    {
        var response = await _client.GetAsync("/self");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_Ready_Returns200()
    {
        var response = await _client.GetAsync("/ready");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}