using ProductCatalog.Api.Dtos;

namespace ProductCatalog.Api.Services;

public interface IProductService
{
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<PagedResponse<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct);
    Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}