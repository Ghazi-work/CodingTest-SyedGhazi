using FluentValidation;
using MediatR;

namespace ProductsApi.Features.Products.CreateProduct;

public static class CreateProductEndpoint
{
    public static void MapCreateProductEndpoint(this WebApplication app)
    {
        app.MapPost("/api/products", async (
            CreateProductRequest request,
            IValidator<CreateProductRequest> validator,
            IMediator mediator) =>
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
                return Results.ValidationProblem(validation.ToDictionary());

            var command = new CreateProductCommand(request.Name, request.Colour, request.Price);
            var id = await mediator.Send(command);

            return Results.Created($"/api/products/{id}", new { id });
        }).RequireAuthorization();
    }
}
