using ProductCatalog.Api.Domain.Enums;

namespace ProductCatalog.Api.Dtos;

public abstract class ProductRequestBase
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Sku { get; set; } = null!;
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }
    public int Quantity { get; set; }
    public double Weight { get; set; }
    public float Rating { get; set; }
    public bool IsAvailable { get; set; }
    public string? Color { get; set; }
    public ProductCategory Category { get; set; }
    public ProductStatus Status { get; set; }
    public string[] Tags { get; set; } = [];
    public string[] Materials { get; set; } = [];
    public CountryOfOrigin CountryOfOrigin { get; set; }
    public string Brand { get; set; } = null!;
    public bool HasHonestSign { get; set; }
}
