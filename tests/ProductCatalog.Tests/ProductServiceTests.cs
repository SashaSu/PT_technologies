using Moq;
using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Domain.Enums;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Repositories;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repo = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_repo.Object);
    }

    private static Product CreateProduct(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = "Кружка",
        Sku = "MUG-001",
        Price = 500,
        Quantity = 10,
        Brand = "Brand",
        Category = ProductCategory.Home,
        Status = ProductStatus.Published,
        CountryOfOrigin = CountryOfOrigin.Russia,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static CreateProductRequest CreateRequest() => new()
    {
        Name = "Новая кружка",
        Sku = "MUG-002",
        Price = 700,
        Quantity = 5,
        Brand = "NewBrand",
        Category = ProductCategory.Home,
        Status = ProductStatus.Draft,
        CountryOfOrigin = CountryOfOrigin.China
    };

    [Fact]
    public async Task GetByIdAsync_ProductExists()
    {
        var product = CreateProduct();
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        var result = await _service.GetByIdAsync(product.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NoProduct()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPage()
    {
        var items = new List<Product> { CreateProduct(), CreateProduct() };
        _repo.Setup(r => r.GetPagedAsync(2, 2, It.IsAny<CancellationToken>()))
             .ReturnsAsync((items, 7));

        var result = await _service.GetPagedAsync(2, 2, CancellationToken.None);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Page);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(7, result.TotalCount);
    }

    [Fact]
    public async Task CreateAsync_SavesProduct()
    {
        var request = CreateRequest();

        var result = await _service.CreateAsync(request, CancellationToken.None);

        _repo.Verify(r => r.AddAsync(
            It.Is<Product>(p => p.Sku == request.Sku && p.Name == request.Name),
            It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(request.Name, result.Name);
    }

    [Fact]
    public async Task UpdateAsync_ProductExists()
    {
        var product = CreateProduct();
        var originalCreatedAt = product.CreatedAt;
        var request = new UpdateProductRequest
        {
            Name = "Обновлённое имя",
            Sku = "NEW-SKU",
            Price = 999,
            Brand = "Other"
        };
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        var result = await _service.UpdateAsync(product.Id, request, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Обновлённое имя", result.Name);
        Assert.Equal(999, result.Price);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(originalCreatedAt, result.CreatedAt);
        _repo.Verify(r => r.UpdateAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NoProduct()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);

        var result = await _service.UpdateAsync(Guid.NewGuid(), new UpdateProductRequest(), CancellationToken.None);

        Assert.Null(result);
        _repo.Verify(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ProductExists()
    {
        var product = CreateProduct();
        _repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
             .ReturnsAsync(product);

        var result = await _service.DeleteAsync(product.Id, CancellationToken.None);

        Assert.True(result);
        _repo.Verify(r => r.DeleteAsync(product, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NoProduct()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
             .ReturnsAsync((Product?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(result);
        _repo.Verify(r => r.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
}