internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        
        // Add the authentication services to DI
        //builder.AddDefaultAuthentication();

        // Pooling is disabled because of the following error:
        // Unhandled exception. System.InvalidOperationException:
        // The DbContext of type 'OrderingContext' cannot be pooled because it does not have a public constructor accepting a single parameter of type DbContextOptions or has more than one constructor.
        //services.AddDbContext<OrderingContext>(options =>
        //{
        //    options.Use(builder.Configuration.GetConnectionString("orderingdb"));
        //});
        //builder.EnrichNpgsqlDbContext<OrderingContext>();


        //builder.AddSqlServerDbContext<OrderingContext>("orderingdb");

        //services.AddMigration<OrderingContext, OrderingContextSeed>();

        // Add the integration services that consume the DbContext

        //services.AddScoped<IBuyerRepository, BuyerRepository>();
        //services.AddScoped<IOrderRepository, OrderRepository>();
        //services.AddScoped<IRequestManager, RequestManager>();
    }

    //private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
    //{
    //    eventBus.AddSubscription<GracePeriodConfirmedIntegrationEvent, GracePeriodConfirmedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderStockConfirmedIntegrationEvent, OrderStockConfirmedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderStockRejectedIntegrationEvent, OrderStockRejectedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();
    //    eventBus.AddSubscription<OrderPaymentSucceededIntegrationEvent, OrderPaymentSucceededIntegrationEventHandler>();
    //}
}
