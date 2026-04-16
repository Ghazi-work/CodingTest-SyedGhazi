using Microsoft.EntityFrameworkCore;
using ProductsApi.Common.Persistence;
using ProductsApi.Features.Products.CreateProduct;

namespace ProductsApi.UnitTests;

public class CreateProductHandlerTests
{
    private static ProductsDbContext CreateInMemoryContext() =>
        new(new DbContextOptionsBuilder<ProductsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    [Fact]
    public async Task Handle_SavesProduct_AndReturnsValidGuid()
    {
        await using var db = CreateInMemoryContext();
        var handler = new CreateProductHandler(db);
        var command = new CreateProductCommand("Widget", "Red", 9.99m);

        var id = await handler.Handle(command, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        var saved = await db.Products.FindAsync(id);
        Assert.NotNull(saved);
        Assert.Equal("Widget", saved.Name);
        Assert.Equal("Red", saved.Colour);
        Assert.Equal(9.99m, saved.Price);
        Assert.NotEqual(default, saved.CreatedAt);
    }
}
