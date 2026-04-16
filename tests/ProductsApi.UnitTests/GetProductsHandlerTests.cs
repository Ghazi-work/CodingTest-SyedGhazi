using Microsoft.EntityFrameworkCore;
using ProductsApi.Common.Models;
using ProductsApi.Common.Persistence;
using ProductsApi.Features.Products.GetProducts;

namespace ProductsApi.UnitTests;

public class GetProductsHandlerTests
{
    private static ProductsDbContext CreateInMemoryContext() =>
        new(new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private static async Task<ProductsDbContext> CreateSeededContext(params Product[] products)
    {
        var db = CreateInMemoryContext();
        db.Products.AddRange(products);
        await db.SaveChangesAsync();
        return db;
    }

    [Fact]
    public async Task Handle_WithNoColour_ReturnsAllProducts()
    {
        await using var db = await CreateSeededContext(
            new Product { Id = Guid.NewGuid(), Name = "Widget A", Colour = "Red",  Price = 5.99m, CreatedAt = DateTimeOffset.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Widget B", Colour = "Blue", Price = 7.99m, CreatedAt = DateTimeOffset.UtcNow }
        );

        var result = await new GetProductsHandler(db).Handle(new GetProductsQuery(null), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_WithColourFilter_ReturnsOnlyMatchingProducts()
    {
        await using var db = await CreateSeededContext(
            new Product { Id = Guid.NewGuid(), Name = "Widget A", Colour = "Red",  Price = 5.99m, CreatedAt = DateTimeOffset.UtcNow },
            new Product { Id = Guid.NewGuid(), Name = "Widget B", Colour = "Blue", Price = 7.99m, CreatedAt = DateTimeOffset.UtcNow }
        );

        var result = await new GetProductsHandler(db).Handle(new GetProductsQuery("Red"), CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Red", result[0].Colour);
    }

    [Fact]
    public async Task Handle_WithColourFilter_IsCaseInsensitive()
    {
        await using var db = await CreateSeededContext(
            new Product { Id = Guid.NewGuid(), Name = "Widget A", Colour = "Red", Price = 5.99m, CreatedAt = DateTimeOffset.UtcNow }
        );

        var result = await new GetProductsHandler(db).Handle(new GetProductsQuery("RED"), CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task Handle_WithNonMatchingColour_ReturnsEmptyList()
    {
        await using var db = await CreateSeededContext(
            new Product { Id = Guid.NewGuid(), Name = "Widget A", Colour = "Red", Price = 5.99m, CreatedAt = DateTimeOffset.UtcNow }
        );

        var result = await new GetProductsHandler(db).Handle(new GetProductsQuery("Green"), CancellationToken.None);

        Assert.Empty(result);
    }
}
