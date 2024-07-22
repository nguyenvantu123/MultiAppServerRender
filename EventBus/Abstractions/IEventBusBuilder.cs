using Microsoft.Extensions.DependencyInjection;

namespace MultiAppServer.EventBus.Abstractions;

public interface IEventBusBuilder
{
    public IServiceCollection Services { get; }
}
