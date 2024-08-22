using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BlazorIdentity.Repositories;
using Aspire.StackExchange.Redis;

namespace BlazorIdentity.Users.Extensions
{
    public static class Extension
    {

        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            // Add the authentication services to DI


        }

        //private static void AddEventBusSubscriptions(this IEventBusBuilder eventBus)
        //{

        //}
    }
}
