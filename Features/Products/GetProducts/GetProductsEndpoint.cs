using MediatR;

namespace ProductsApi.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static void MapGetProductsEndpoint(this WebApplication app)
    {
        app.MapGet("/api/products", async (string? colour, IMediator mediator) =>
        {
            var products = await mediator.Send(new GetProductsQuery(colour));
            return Results.Ok(products);
        }).RequireAuthorization();
    }
}
