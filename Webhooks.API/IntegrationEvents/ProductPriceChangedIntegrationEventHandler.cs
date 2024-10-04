using MultiAppServer.EventBus.Abstractions;

namespace Webhooks.API.IntegrationEvents;

public class ProductPriceChangedIntegrationEventHandler : IIntegrationEventHandler<ProductPriceChangedIntegrationEvent>
{
    public Task Handle(ProductPriceChangedIntegrationEvent @event)
    {
        return Task.CompletedTask;
    }
}
