using System.Net;
using System.Text.Json;

namespace ProductsApi.IntegrationTests;

public class HealthEndpointTests : IClassFixture<ProductsApiFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(ProductsApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetHealth_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);
        var status = doc.RootElement.GetProperty("status").GetString();

        Assert.Equal("healthy", status);
    }
}
