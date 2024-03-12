using BlazorWeb.Request.Identity;
using BlazorWeb.Response.Identity;
using BlazorWeb.Response.Role;
using BlazorWeb.Response.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Polly.Retry;
using Polly;
using Refit;
using System.Net;
using Microsoft.Extensions.Configuration;
using BlazorWeb.Wrapper;
using System.Net.Sockets;

namespace BlazorWeb.Services.BffApiClients
{
    public interface IBffApiClients
    {
        [Post("/identity/token")]
        Task<ResultBase<TokenResponse>> LoginAsync([Body] TokenRequest tokenRequest);

        [Post("/identity/refreshToken")]
        Task<ResultBase<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest);

        [Get("/user/me")]
        Task<ResultBase<UserProfileResponse>> UserProfile([Header("Authorization")] string authorization);

        [Post("/user/roles")]
        Task<ResultBase<UserRolesResponse>> GetRolesAsync();

        [Post("/roles")]
        Task<ResultBase<List<RoleResponse>>> GetAllRole();

    }

    //public class BffApiClients : IBffApiClients
    //{

    //    public readonly IBffApiClients Client;

    //    private static readonly IReadOnlyCollection<HttpStatusCode> HttpStatusCodesWorthRetrying =
    //        new List<HttpStatusCode>
    //        {
    //            HttpStatusCode.RequestTimeout, // 408
    //            HttpStatusCode.InternalServerError, // 500
    //            HttpStatusCode.BadGateway, // 502
    //            HttpStatusCode.ServiceUnavailable, // 503
    //            HttpStatusCode.GatewayTimeout, // 504
    //            HttpStatusCode.Unauthorized
    //        };

    //    public static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = Policy
    //        .Handle<HttpRequestException>()
    //        .OrResult<HttpResponseMessage>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
    //        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));

    //    public BffApiClients()
    //    {
    //        var httpClient = new HttpClient
    //        {
    //            BaseAddress = new Uri("http://blazorwebapi.bff")
    //        };
    //        Client = RestService.For<IBffApiClients>(httpClient);
    //    }

    //    public async Task<ResultBase<List<RoleResponse>>> GetAllRole()
    //    {
    //        return await Client.GetAllRole();
    //    }

    //    public async Task<ResultBase<UserRolesResponse>> GetRolesAsync()
    //    {
    //        return await Client.GetRolesAsync();
    //    }

    //    public async Task<ResultBase<TokenResponse>> LoginAsync([Body] TokenRequest tokenRequest)
    //    {
    //        return await Client.LoginAsync(tokenRequest);

    //    }

    //    public async Task<ResultBase<TokenResponse>> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest)
    //    {
    //        return await Client.RefreshTokenAsync(tokenRequest);

    //    }

    //    public async Task<ResultBase<UserProfileResponse>> UserProfile([Header("Authorization")] string authorization)
    //    {
    //        return await Client.UserProfile(authorization);

    //    }
    //}
}
