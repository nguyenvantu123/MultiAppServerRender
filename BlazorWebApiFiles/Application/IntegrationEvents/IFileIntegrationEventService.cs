using MultiAppServer.EventBus.Events;

namespace BlazorIdentityFiles.Application.IntegrationEvents;

public interface IFileIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent evt);
}
