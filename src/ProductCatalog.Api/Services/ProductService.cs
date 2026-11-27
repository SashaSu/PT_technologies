using DefaultNamespace;
using ProductCatalog.Api.Converters;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IEventPublisher _events;

    public ProductService(IProductRepository repository, IEventPublisher events)
    {
        _repository = repository;
        _events = events;
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => (await _repository.GetByIdAsync(id, ct))?.ToDto();

    public async Task<PagedResponse<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var (items, total) = await _repository.GetPagedAsync(page, pageSize, ct);
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
        await _repository.AddAsync(product, ct);

        var dto = product.ToDto();
        await _events.PublishAsync(new ProductChangedEvent
        {
            Type = "Created",
            ProductId = dto.Id,
            Product = dto,
            OccurredAt = DateTimeOffset.UtcNow
        }, ct);

        return dto;
    }

    public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
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

        await _repository.UpdateAsync(product, ct);
        var dto = product.ToDto();
        await _events.PublishAsync(new ProductChangedEvent
        {
            Type = "Updated",
            ProductId = dto.Id,
            Product = dto,
            OccurredAt = DateTimeOffset.UtcNow
        }, ct);

        return dto;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(id, ct);
        if (product is null) return false;

        await _repository.DeleteAsync(product, ct);
        await _events.PublishAsync(new ProductChangedEvent
        {
            Type = "Deleted",
            ProductId = id,
            Product = null,
            OccurredAt = DateTimeOffset.UtcNow
        }, ct);

        return true;
    }
}