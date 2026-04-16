using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProductsApi.IntegrationTests;

public class CreateProductEndpointTests : IClassFixture<ProductsApiFactory>
{
    private readonly ProductsApiFactory _factory;
    private readonly IConfiguration _config;

    public CreateProductEndpointTests(ProductsApiFactory factory)
    {
        _factory = factory;
        _config = factory.Services.GetRequiredService<IConfiguration>();
    }

    [Fact]
    public async Task PostProduct_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            Name = "Widget",
            Colour = "Red",
            Price = 9.99
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostProduct_WithValidTokenAndBody_Returns201WithId()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenHelper.GenerateToken(_config));

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            Name = "Widget",
            Colour = "Red",
            Price = 9.99
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        var id = body.GetProperty("id").GetGuid();
        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task PostProduct_WithValidTokenAndInvalidBody_Returns400()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenHelper.GenerateToken(_config));

        var response = await client.PostAsJsonAsync("/api/products", new
        {
            Name = "",
            Colour = "Red",
            Price = 9.99
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
