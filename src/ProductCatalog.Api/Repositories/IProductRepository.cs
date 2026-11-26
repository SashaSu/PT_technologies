using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task UpdateAsync(Product product, CancellationToken ct);
    Task DeleteAsync(Product product, CancellationToken ct);
}