using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public class ProductService(IProductRepository repository) : IProductService
{
    public Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct)
        => throw new NotImplementedException();

    public Task<PagedResponse<ProductDto>> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        => throw new NotImplementedException();

    public Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct)
        => throw new NotImplementedException();

    public Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct)
        => throw new NotImplementedException();

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct)
        => throw new NotImplementedException();
}