
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using AutoMapper;
using BlazorIdentity.Data;
using BlazorIdentityApi.Mappers.Configuration;
using eShop.ServiceDefaults;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddDefaultAuthentication();

        //builder.AddRabbitMqEventBus("EventBus").AddEventBusSubscriptions();

        //builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

        //builder.Services.AddMigration<WebhooksContext>();

        //builder.Services.AddTransient<IGrantUrlTesterService, GrantUrlTesterService>();
        //builder.Services.AddTransient<IWebhooksRetriever, WebhooksRetriever>();
        //builder.Services.AddTransient<IWebhooksSender, WebhooksSender>();
    }

    public static IMapperConfigurationBuilder AddAdminAspNetIdentityMapping(this IServiceCollection services)
    {
        var builder = new MapperConfigurationBuilder();

        services.AddSingleton<AutoMapper.IConfigurationProvider>(sp => new MapperConfiguration(cfg =>
        {
            foreach (var profileType in builder.ProfileTypes)
                cfg.AddProfile(profileType);
        }));

        services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));

        return builder;
    }

    //private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    //{
    //    eventBus.AddSubscription<ProductPriceChangedIntegrationEvent, ProductPriceChangedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderStatusChangedToShippedIntegrationEvent, OrderStatusChangedToShippedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderStatusChangedToPaidIntegrationEvent, OrderStatusChangedToPaidIntegrationEventHandler>();
    //}
}
