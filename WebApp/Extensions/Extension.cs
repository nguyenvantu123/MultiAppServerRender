namespace WebApp.Extensions
{
    public static class Extension
    {

        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            // Add the authentication services to DI
            //builder.AddRabbitMqEventBus("eventbus")
            //       .AddEventBusSubscriptions();

            //builder.AddRedisClient("redis");

        }

        //private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        //{
        //    //eventBus.AddSubscription<UserProfileIntegrationEvent, UserProfileEventHandler>();
        //}
    }
}
