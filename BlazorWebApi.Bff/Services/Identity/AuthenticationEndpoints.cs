using BlazorWebApi.Bff.Wrapper;
using BlazorWebApi.Bff.ApiClients;
using MediatR;
using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
=======
//using IResult = BlazorWebApi.Bff.Wrapper.IResult;
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b

namespace BlazorWebApi.Bff.Services.Identity
{
    public static class IdentityEndpoints
    {

        public static IEndpointRouteBuilder AddIdentityEndpoint(this IEndpointRouteBuilder builder)
        {
            var login = builder.MapGroup("/identity");

            login.MapPost("/token", async ([FromBody] LoginRequest loginRequest, [FromServices] IIdentityApiClient identityApiClient) => await LoginAsync(identityApiClient, loginRequest));


            login.MapPost("/refreshToken", async ([FromBody] RefreshRequest refreshTokenRequest, [FromServices] IIdentityApiClient identityApiClient) => await RefreshToken(identityApiClient, refreshTokenRequest));

            return builder;
        }

        private static async Task<ResultBase<TokenResponse>> LoginAsync(IIdentityApiClient identityApiClient, LoginRequest loginRequest)
        {
            var result = await identityApiClient.LoginAsync(loginRequest);

            return result;
        }

<<<<<<< HEAD
        private static async Task<dynamic> RefreshToken(IIdentityApiClient identityApiClient, RefreshRequest refreshTokenRequest)
=======
        private static async Task<ResultBase<TokenResponse>> RefreshToken(IIdentityApiClient identityApiClient, RefreshRequest refreshTokenRequest)
>>>>>>> 3c6e47b79da1d67715f3c930762656f0a6a8fe2b
        {
            var result = await identityApiClient.RefreshTokenAsync(refreshTokenRequest);

            return result;
        }
    }

}
