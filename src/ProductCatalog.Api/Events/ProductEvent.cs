using ProductCatalog.Api.Dtos;

public class ProductChangedEvent
{
    public string Type { get; set; } = null!; 
    public Guid ProductId { get; set; }
    public ProductDto? Product { get; set; }   
    public DateTimeOffset OccurredAt { get; set; }
}