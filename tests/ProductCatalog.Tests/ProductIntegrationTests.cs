using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using ProductCatalog.Api.Domain.Enums;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Tests;
using Xunit;

public class ProductIntegrationTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public ProductIntegrationTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private static CreateProductRequest NewRequest(string sku) => new()
    {
        Name = "Кружка",
        Sku = sku,
        Price = 500,
        Quantity = 10,
        Weight = 0.4,
        Rating = 4.5f,
        IsAvailable = true,
        Category = ProductCategory.Home,
        Status = ProductStatus.Published,
        CountryOfOrigin = CountryOfOrigin.Russia,
        Brand = "Тест"
    };

    private async Task<ProductDto> CreateProduct(string sku)
    {
        var response = await _client.PostAsJsonAsync("/api/products", NewRequest(sku), Json);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var product = await response.Content.ReadFromJsonAsync<ProductDto>(Json);
        return product!;
    }

    [Fact]
    public async Task Create_ThenGetById_ReturnsSameProduct()
    {
        var created = await CreateProduct("SKU-GET-1");

        var response = await _client.GetAsync($"/api/products/{created.Id}");
        var loaded = await response.Content.ReadFromJsonAsync<ProductDto>(Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("SKU-GET-1", loaded!.Sku);
    }

    [Fact]
    public async Task Create_WithNegativePrice_ReturnsBadRequest()
    {
        var request = NewRequest("SKU-BAD-1");
        request.Price = -5;

        var response = await _client.PostAsJsonAsync("/api/products", request, Json);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/products/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ChangesPrice()
    {
        var created = await CreateProduct("SKU-UPD-1");

        var update = new UpdateProductRequest
        {
            Name = "Кружка новая",
            Sku = "SKU-UPD-1",
            Price = 999,
            Quantity = 10,
            Weight = 0.4,
            Rating = 4.5f,
            Category = ProductCategory.Home,
            Status = ProductStatus.Published,
            CountryOfOrigin = CountryOfOrigin.Russia,
            Brand = "Тест"
        };

        var response = await _client.PutAsJsonAsync($"/api/products/{created.Id}", update, Json);
        var updated = await response.Content.ReadFromJsonAsync<ProductDto>(Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(999, updated!.Price);
    }

    [Fact]
    public async Task Delete_ThenGetById_ReturnsNotFound()
    {
        var created = await CreateProduct("SKU-DEL-1");

        var deleteResponse = await _client.DeleteAsync($"/api/products/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/products/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsRequestedPage()
    {
        await CreateProduct("SKU-PAGE-1");
        await CreateProduct("SKU-PAGE-2");
        await CreateProduct("SKU-PAGE-3");

        var response = await _client.GetAsync("/api/products?page=1&pageSize=2");
        var page = await response.Content.ReadFromJsonAsync<PagedResponse<ProductDto>>(Json);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(2, page!.Items.Count);
        Assert.True(page.TotalCount >= 3);
    }
}