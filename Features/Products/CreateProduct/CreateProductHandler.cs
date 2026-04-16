using MediatR;
using ProductsApi.Common.Models;
using ProductsApi.Common.Persistence;

namespace ProductsApi.Features.Products.CreateProduct;

public record CreateProductCommand(string Name, string Colour, decimal Price) : IRequest<Guid>;

public class CreateProductHandler(ProductsDbContext db) : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Colour = request.Colour,
            Price = request.Price,
            CreatedAt = DateTimeOffset.UtcNow
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
