using MultiAppServer.EventBus.Abstractions;
using MultiAppServer.EventBus.Events;
using WebApp.Events;

namespace WebApp.EventHandling;

public class UpdateProfileEventHandler(
    IEventBus eventBus,
    ILogger<UpdateProfileEventHandler> logger) :
    IIntegrationEventHandler<UpdateProfileEvent>
{
    public async Task Handle(UpdateProfileEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        // Business feature comment:
        // When OrderStatusChangedToStockConfirmed Integration Event is handled.
        // Here we're simulating that we'd be performing the payment against any payment gateway
        // Instead of a real payment we just take the env. var to simulate the payment 
        // The payment can be successful or it can fail


        //logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", orderPaymentIntegrationEvent.Id, orderPaymentIntegrationEvent);

        await eventBus.PublishAsync(@event);
    }
}
