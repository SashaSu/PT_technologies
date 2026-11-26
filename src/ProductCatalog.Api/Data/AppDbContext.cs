using Microsoft.EntityFrameworkCore;
using ProductCatalog.Api.Domain;

namespace ProductCatalog.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.Sku).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.Sku).IsUnique();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.Category).HasConversion<string>(); 
            e.Property(x => x.Status).HasConversion<string>();
            
        });
    }
}