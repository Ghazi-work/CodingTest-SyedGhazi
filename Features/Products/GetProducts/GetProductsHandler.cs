using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductsApi.Common.Persistence;

namespace ProductsApi.Features.Products.GetProducts;

public record ProductDto(Guid Id, string Name, string Colour, decimal Price, DateTimeOffset CreatedAt);

public record GetProductsQuery(string? Colour) : IRequest<List<ProductDto>>;

public class GetProductsHandler(ProductsDbContext db) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Colour))
            query = query.Where(p => p.Colour.ToLower() == request.Colour.ToLower());

        return await query
            .Select(p => new ProductDto(p.Id, p.Name, p.Colour, p.Price, p.CreatedAt))
            .ToListAsync(cancellationToken);
    }
}
