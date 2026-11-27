using ProductCatalog.Api.Converters;
using ProductCatalog.Api.Repositories;

namespace ProductCatalog.Api.Services;

public class ExportService : IExportService
{
    private readonly IProductRepository _repo;
    private readonly HttpClient _http;
    private readonly string _url;

    public ExportService(IProductRepository repo, HttpClient http, IConfiguration config)
    {
        _repo = repo;
        _http = http;
        _url = config["Export:Url"] ?? throw new InvalidOperationException("Не задан Export:Url");
    }

    public async Task<int> ExportAllAsync(CancellationToken ct)
    {
        var exported = 0;
        var page = 1;
        var pageSize = 100;

        while (true)
        {
            var (items, total) = await _repo.GetPagedAsync(page, pageSize, ct);
            if (items.Count == 0)
            {
                break;
            }

            var dtos = items.Select(p => p.ToDto()).ToList();

            var response = await _http.PostAsJsonAsync(_url, dtos, ct);
            response.EnsureSuccessStatusCode();

            exported += dtos.Count;
            page++;
        }

        return exported;
    }
}