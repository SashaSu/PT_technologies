using FluentValidation;
using ProductCatalog.Api.Dtos;

public abstract class ProductRequestValidator<T> : AbstractValidator<T> where T : ProductRequestBase
{
    protected ProductRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Sku).NotEmpty();
        RuleFor(x => x.Brand).NotEmpty();

        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.OldPrice).GreaterThan(0).When(x => x.OldPrice.HasValue);
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Weight).GreaterThan(0);
        RuleFor(x => x.Rating).InclusiveBetween(0, 5);

        RuleFor(x => x.Category).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.CountryOfOrigin).IsInEnum();

        RuleFor(x => x.Tags).NotNull();
        RuleFor(x => x.Materials).NotNull();
    }
}

public class CreateProductRequestValidator : ProductRequestValidator<CreateProductRequest> { }

public class UpdateProductRequestValidator : ProductRequestValidator<UpdateProductRequest> { }