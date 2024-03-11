using BlazorWebApi.Bff.Wrapper;
using BlazorWebApi.Bff.ApiClients;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        private static async Task<ResultBase<TokenResponse>> RefreshToken(IIdentityApiClient identityApiClient, RefreshRequest refreshTokenRequest)
        {
            var result = await identityApiClient.RefreshTokenAsync(refreshTokenRequest);

            return result;
        }
    }

}
