using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Dtos;

[ApiController]
[Route("mock/import")]
public class MockImportController : ControllerBase
{
    private readonly ILogger<MockImportController> _logger;

    public MockImportController(ILogger<MockImportController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public IActionResult Receive(List<ProductDto> products)
    {
        _logger.LogInformation("Мок-сервис получил товаров: {Count}", products.Count);
        return Ok(new { received = products.Count });
    }
}