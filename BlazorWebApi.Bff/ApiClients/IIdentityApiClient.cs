using BlazorWebApi.Bff.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace BlazorWebApi.Bff.ApiClients
{
    public interface IIdentityApiClient
    {
        [Post("/identity/token")]
        public Task<object> LoginAsync([FromBody] LoginRequest loginRequest);

        [Post("/identity/refreshToken")]
        public Task<object> RefreshTokenAsync([FromBody] RefreshRequest refreshRequest);
    }


    public record LoginRequest(string username, string password, int clientId);

    public record TokenResponse(string accessToken, string refreshToken, DateTime expireIn);

    public record RefreshRequest(string accessToken, string refreshToken);

}
