using ProductCatalog.Api.Data;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        => throw new NotImplementedException();

    public Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
        => throw new NotImplementedException();

    public Task AddAsync(Product product, CancellationToken ct)
        => throw new NotImplementedException();

    public Task UpdateAsync(Product product, CancellationToken ct)
        => throw new NotImplementedException();

    public Task DeleteAsync(Product product, CancellationToken ct)
        => throw new NotImplementedException();
}