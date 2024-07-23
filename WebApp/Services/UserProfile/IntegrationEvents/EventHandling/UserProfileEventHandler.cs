
using MultiAppServer.EventBus.Abstractions;

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public class UserProfileEventHandler(
    UserNotificationService orderStatusNotificationService,
    ILogger<UserProfileEventHandler> logger)
    : IIntegrationEventHandler<UserProfileIntegrationEvent>
{
    public async Task Handle(UserProfileIntegrationEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);
        //await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.UserId);
    }
}
