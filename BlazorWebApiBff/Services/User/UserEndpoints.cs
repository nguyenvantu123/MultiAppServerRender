using BlazorWebApi.Bff.ApiClients;
using BlazorWebApi.Bff.Wrapper;
using MediatR;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace BlazorWebApi.Bff.Services.User
{
    public static class UserEndpoints
    {

        public static IEndpointRouteBuilder AddUserEndpoint(this IEndpointRouteBuilder builder)
        {
            var user = builder.MapGroup("/user");


            user.MapGet("/me", async ([FromServices] IUserApiClient userApiClient) => await GetUserProfile(userApiClient));


            user.MapGet("", async ([FromBody] GetAllUserQuery getAllUserQuery, [FromServices] IUserApiClient userApiClient) => await GetAllUser(userApiClient, getAllUserQuery));

            return builder;
        }

        private static async Task<object> GetUserProfile(IUserApiClient identityApiClient)
        {

            var result = await identityApiClient.UserProfile();

            return result;
        }

        private static async Task<object> GetAllUser(IUserApiClient identityApiClient, [FromBody] GetAllUserQuery getAllUserQuery)
        {

            var result = await identityApiClient.GetAllUser(getAllUserQuery);

            return result;
        }
    }

}
