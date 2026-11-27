using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using ProductCatalog.Api.Domain;
using ProductCatalog.Api.Repositories;
using ProductCatalog.Api.Services;
using Xunit;

public class ExportServiceTests
{
    private class StubHandler : HttpMessageHandler
    {
        public int Calls;

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }
    }

    [Fact]
    public async Task ExportAllAsync_SendsAllProductsAndReturnsCount()
    {
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "A", Sku = "A", Brand = "B" },
            new() { Id = Guid.NewGuid(), Name = "B", Sku = "B", Brand = "B" }
        };

        var repo = new Mock<IProductRepository>();
        repo.Setup(r => r.GetPagedAsync(1, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((products, 2));
        repo.Setup(r => r.GetPagedAsync(2, 100, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<Product>(), 2));

        var handler = new StubHandler();
        var http = new HttpClient(handler);

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Export:Url"] = "http://localhost/mock/import"
            })
            .Build();

        var service = new ExportService(repo.Object, http, config);

        var exported = await service.ExportAllAsync(CancellationToken.None);

        Assert.Equal(2, exported);
        Assert.Equal(1, handler.Calls);
    }
}