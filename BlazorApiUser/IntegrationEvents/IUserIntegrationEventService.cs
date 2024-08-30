using MultiAppServer.EventBus.Events;

namespace BlazorApiUser.IntegrationEvents;

public interface IUserIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent evt);
}
