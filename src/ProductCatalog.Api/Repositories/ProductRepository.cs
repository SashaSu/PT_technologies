using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Data;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Repositories;

public class ProductRepository(AppDbContext db) : IProductRepository
{
    public Task<Product?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<(List<Product> Items, int Total)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var query = db.Products.AsNoTracking();
        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(p => p.CreatedAt).ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        return (items, total);
    }

    public async Task AddAsync(Product product, CancellationToken ct)
    {
        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
    }

    public Task UpdateAsync(Product product, CancellationToken ct) => db.SaveChangesAsync(ct);


    public async Task DeleteAsync(Product product, CancellationToken ct)
    {
        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
    }
}