namespace ProductCatalog.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Api.Services;

[ApiController]
[Route("api/export")]
public class ExportController : ControllerBase
{
    private readonly IExportService _exportService;

    public ExportController(IExportService exportService)
    {
        _exportService = exportService;
    }

    [HttpPost]
    public async Task<IActionResult> Export(CancellationToken ct)
    {
        try
        {
            var count = await _exportService.ExportAllAsync(ct);
            return Ok(new { exported = count });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new { error = "Не удалось отправить данные во внешний сервис" });
        }
    }
}