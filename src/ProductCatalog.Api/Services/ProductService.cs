using ProductCatalog.Api.Converters;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => (await repository.GetByIdAsync(id, ct))?.ToDto();

    public async Task<PagedResponse<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var (items, total) = await repository.GetPagedAsync(page, pageSize, ct);
        return new PagedResponse<ProductDto>
        {
            Items = items.Select(p => p.ToDto()).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = total
        };
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct)
    {
        var product = request.ToEntity();
        await repository.AddAsync(product, ct);
        return product.ToDto(); 
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        if (product is null) return null;

        product.Name = request.Name;
        product.Description = request.Description;
        product.Sku = request.Sku;
        product.Price = request.Price;
        product.OldPrice = request.OldPrice;
        product.Quantity = request.Quantity;
        product.Weight = request.Weight;
        product.Rating = request.Rating;
        product.IsAvailable = request.IsAvailable;
        product.Color = request.Color;
        product.Category = request.Category;
        product.Status = request.Status;
        product.Tags = request.Tags;
        product.Materials = request.Materials;
        product.CountryOfOrigin = request.CountryOfOrigin;
        product.Brand = request.Brand;
        product.HasHonestSign = request.HasHonestSign;

        await repository.UpdateAsync(product, ct);
        return product.ToDto();
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct);
        if (product is null) return false;

        await repository.DeleteAsync(product, ct);
        return true;
    }
}