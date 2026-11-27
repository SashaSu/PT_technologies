using FluentValidation.TestHelper;
using ProductCatalog.Api.Dtos;
using Xunit;

public class CreateProductRequestValidatorTests
{
    private readonly CreateProductRequestValidator _validator = new();

    private static CreateProductRequest ValidRequest() => new()
    {
        Name = "Кружка", 
        Sku = "MUG-1",
        Price = 500, 
        Quantity = 1,
        Weight = 0.4, 
        Rating = 4, 
        Brand = "Тест"
    };

    [Fact]
    public void ValidRequest_HasNoErrors()
    {
        var result = _validator.TestValidate(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void NegativePrice_HasError()
    {
        var request = ValidRequest();
        request.Price = -5;

        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }
    
}