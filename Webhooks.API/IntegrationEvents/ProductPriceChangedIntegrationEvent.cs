using MultiAppServer.EventBus.Events;

namespace Webhooks.API.IntegrationEvents;

public record ProductPriceChangedIntegrationEvent(int ProductId, decimal NewPrice, decimal OldPrice) : IntegrationEvent;
