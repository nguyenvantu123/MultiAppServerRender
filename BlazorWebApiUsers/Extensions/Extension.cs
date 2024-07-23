using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using BlazorWebApi.Repositories;
using Aspire.StackExchange.Redis;

namespace BlazorWebApi.Users.Extensions
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
