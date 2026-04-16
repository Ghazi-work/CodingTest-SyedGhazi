using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductsApi.Common.Models;
using ProductsApi.Common.Persistence;

namespace ProductsApi.IntegrationTests;

public class GetProductsEndpointTests : IClassFixture<ProductsApiFactory>
{
    private readonly ProductsApiFactory _factory;
    private readonly IConfiguration _config;

    public GetProductsEndpointTests(ProductsApiFactory factory)
    {
        _factory = factory;
        _config = factory.Services.GetRequiredService<IConfiguration>();
    }

    [Fact]
    public async Task GetProducts_WithoutToken_Returns401()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetProducts_WithValidToken_Returns200WithJsonArray()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenHelper.GenerateToken(_config));

        var response = await client.GetAsync("/api/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
    }

    [Fact]
    public async Task GetProducts_WithColourFilter_ReturnsOnlyMatchingProducts()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            db.Products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = "Blue Widget",
                Colour = "blue",
                Price = 12.99m,
                CreatedAt = DateTimeOffset.UtcNow
            });
            await db.SaveChangesAsync();
        }

        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", TokenHelper.GenerateToken(_config));

        var response = await client.GetAsync("/api/products?colour=blue");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(JsonValueKind.Array, body.ValueKind);
        Assert.True(body.GetArrayLength() >= 1);
        foreach (var item in body.EnumerateArray())
            Assert.Equal("blue", item.GetProperty("colour").GetString(), StringComparer.OrdinalIgnoreCase);
    }
}
