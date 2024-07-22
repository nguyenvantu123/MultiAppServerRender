using MultiAppServer.EventBus.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using EventBus.RabbitMQ;

namespace BlazorWebApi.Users.Extensions
{
    public static class Extension
    {

        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            // Add the authentication services to DI
            builder.AddRabbitMqEventBus("eventbus")
                   .AddEventBusSubscriptions();

            builder.AddRedisClient("redis");
        }

        private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        {

        }
    }
}
