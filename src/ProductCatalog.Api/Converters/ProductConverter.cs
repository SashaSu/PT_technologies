using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Dtos;

namespace ProductCatalog.Api.Converters;

public static class ProductConverter
{
    public static ProductDto ToDto(this Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Sku = p.Sku,
        Price = p.Price,
        OldPrice = p.OldPrice,
        Quantity = p.Quantity,
        Weight = p.Weight,
        Rating = p.Rating,
        IsAvailable = p.IsAvailable,
        Color = p.Color, 
        Category = p.Category,
        Status = p.Status,
        Tags = p.Tags, 
        Materials = p.Materials,
        OrdersCount = p.OrdersCount,
        CountryOfOrigin = p.CountryOfOrigin,
        Brand = p.Brand,
        HasHonestSign = p.HasHonestSign,
        CreatedAt = p.CreatedAt
    };

    public static Product ToEntity(this CreateProductRequest r) => new()
    {
        Id = Guid.NewGuid(),
        Name = r.Name,
        Description = r.Description,
        Sku = r.Sku,
        Price = r.Price,
        OldPrice = r.OldPrice,
        Quantity = r.Quantity,
        Weight = r.Weight,
        Rating = r.Rating,
        IsAvailable = r.IsAvailable,
        Color = r.Color,
        Category = r.Category,
        Status = r.Status,
        Tags = r.Tags,
        Materials = r.Materials,
        CountryOfOrigin = r.CountryOfOrigin,
        Brand = r.Brand,
        HasHonestSign = r.HasHonestSign,
        CreatedAt = DateTimeOffset.UtcNow
    };
}