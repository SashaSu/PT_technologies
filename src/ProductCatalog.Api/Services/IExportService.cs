namespace ProductCatalog.Api.Services;

public interface IExportService
{
    Task<int> ExportAllAsync(CancellationToken ct);
}