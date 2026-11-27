using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ProductCatalog.Api.Dtos;
using ProductCatalog.Api.Services;

[ApiController]
[Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;

    public ProductsController(
        IProductService service,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator)
    {
        _service = service;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 20, CancellationToken ct = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var result = await _service.GetPagedAsync(page, pageSize, ct);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var product = await _service.GetByIdAsync(id, ct);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProductRequest request, CancellationToken ct)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        try
        {
            var product = await _service.CreateAsync(request, ct);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }
        catch (DbUpdateException ex)
        {
            return HandleDbError(ex);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateProductRequest request, CancellationToken ct)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));
        }

        try
        {
            var product = await _service.UpdateAsync(id, request, ct);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }
        catch (DbUpdateException ex)
        {
            return HandleDbError(ex);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var deleted = await _service.DeleteAsync(id, ct);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
    
    private IActionResult HandleDbError(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pg)
        {
            if (pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict(new { error = "Товар с таким артикулом уже существует" });
            }
            if (pg.SqlState == PostgresErrorCodes.StringDataRightTruncation)
            {
                return BadRequest(new { error = "Одно из текстовых полей слишком длинное" });
            }
        }

        return StatusCode(500, new { error = "Ошибка при сохранении" });
    }
    
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
}