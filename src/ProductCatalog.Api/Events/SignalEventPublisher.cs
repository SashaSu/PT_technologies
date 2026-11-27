using DefaultNamespace;
using Microsoft.AspNetCore.SignalR;

public class SignalEventPublisher : IEventPublisher
{
    private readonly IHubContext<ProductHub> _hub;

    public SignalEventPublisher(IHubContext<ProductHub> hub)
    {
        _hub = hub;
    }

    public async Task PublishAsync(ProductChangedEvent productEvent, CancellationToken ct)
    {
        await _hub.Clients.All.SendAsync("ProductChanged", productEvent, ct);
    }
}