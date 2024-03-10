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


            //login.MapPost("/refreshToken", async ([FromBody] RefreshRequest loginRequest, [FromServices] IMediator mediator, CancellationToken cancellationToken) =>
            //{
            //    var result = await mediator.Send(login, cancellationToken);

            //    return Results.Ok(result);
            //});

            return builder;
        }

<<<<<<< HEAD
        private static async Task<dynamic> GetUserProfile(IUserApiClient identityApiClient)
=======
        private static async Task<ResultBase<UserProfile>> GetUserProfile(IUserApiClient identityApiClient)
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
        {

            var result = await identityApiClient.UserProfile();

            return result;
        }
    }

}
