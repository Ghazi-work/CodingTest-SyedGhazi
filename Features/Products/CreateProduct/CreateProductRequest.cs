using FluentValidation;

namespace ProductsApi.Features.Products.CreateProduct;

public record CreateProductRequest(string Name, string Colour, decimal Price);

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Colour).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}
