using MultiAppServer.EventBus.Events;

namespace MultiAppServer.EventBus.Abstractions;

public interface IEventBus
{
    Task PublishAsync(IntegrationEvent @event);
}
