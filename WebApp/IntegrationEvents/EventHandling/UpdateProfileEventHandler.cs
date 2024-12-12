using MultiAppServer.EventBus.Abstractions;
using WebApp.Events;

namespace WebApp.IntegrationEvents.EventHandling;

public class UpdateProfileEventHandler(
    ILogger<UpdateProfileEventHandler> logger) : IIntegrationEventHandler<UpdateProfileEvent>
{
    /// <summary>
    /// Event handler which confirms that the grace period
    /// has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    /// <param name="event">       
    /// </param>
    /// <returns></returns>
    public async Task Handle(UpdateProfileEvent @event)
    {
        logger.LogInformation("Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})", @event.Id, @event);

        //var command = new SetAwaitingValidationOrderStatusCommand(@event.OrderId);

        //logger.LogInformation(
        //    "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
        //    command.GetGenericTypeName(),
        //    nameof(command.OrderNumber),
        //    command.OrderNumber,
        //    command);

        await Task.CompletedTask;
    }
}
