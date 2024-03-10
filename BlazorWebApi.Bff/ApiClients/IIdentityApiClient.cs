using BlazorWebApi.Bff.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IIdentityApiClient
    {
        [Post("/identity/token")]
        public Task<ResultBase<TokenResponse>> LoginAsync([FromBody] LoginRequest loginRequest);

        [AllowAnonymous]
        [Post("/identity/refreshToken")]
        public Task<ResultBase<TokenResponse>> RefreshTokenAsync([FromBody] RefreshRequest refreshRequest);

    }


    public record LoginRequest(string username, string password, int clientId);

    public record TokenResponse(string accessToken, string refreshToken, DateTime expireIn);

    public record RefreshRequest(string accessToken, string refreshToken);

}
