using MultiAppServer.EventBus.Events;

namespace BlazorIdentityFiles.Application.IntegrationEvents;

public interface IUserIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent evt);
}
