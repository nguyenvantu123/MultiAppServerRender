
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using Aspire.MySqlConnector;
using eShop.ServiceDefaults;
using MultiAppServer.EventBus.Abstractions;
using Webhooks.API.Infrastructure;
using Webhooks.API.IntegrationEvents;
using Webhooks.API.Services;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultAuthentication();

        builder.AddMySqlDataSource("webhooksdb");
        builder.AddRabbitMqEventBus("EventBus").AddEventBusSubscriptions();

        //builder.AddSqlServerDbContext<WebhooksContext>("webhooksdb");
        //builder.Services.AddMigration<WebhooksContext>();

        builder.Services.AddTransient<IGrantUrlTesterService, GrantUrlTesterService>();
        builder.Services.AddTransient<IWebhooksRetriever, WebhooksRetriever>();
        builder.Services.AddTransient<IWebhooksSender, WebhooksSender>();
    }

    private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    {
        eventBus.AddSubscription<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
        eventBus.AddSubscription<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
        eventBus.AddSubscription<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
    }
}
