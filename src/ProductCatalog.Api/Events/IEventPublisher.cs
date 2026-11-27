namespace DefaultNamespace;

public interface IEventPublisher
{
    Task PublishAsync(ProductChangedEvent productEvent, CancellationToken ct);
}