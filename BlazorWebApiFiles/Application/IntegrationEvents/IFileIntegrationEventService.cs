using MultiAppServer.EventBus.Events;

namespace BlazorWebApiFiles.Application.IntegrationEvents;

public interface IFileIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);
    Task AddAndSaveEventAsync(IntegrationEvent evt);
}
