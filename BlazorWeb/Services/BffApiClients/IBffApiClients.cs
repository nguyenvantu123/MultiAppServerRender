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
using System;

namespace BlazorWeb.Services.BffApiClients
{
    public interface IBffApiClients
    {
        [Post("/identity/token")]
        Task<HttpResponseMessage> LoginAsync([Body] TokenRequest tokenRequest);

        [Post("/identity/refreshToken")]
        Task<HttpResponseMessage> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest , [Header("Authorization")] string authorization);

        [Get("/user/me")]
        Task<HttpResponseMessage> UserProfile([Header("Authorization")] string authorization);

        [Post("/user/roles")]
        Task<HttpResponseMessage> GetRolesAsync();

        [Post("/roles")]
        Task<HttpResponseMessage> GetAllRole();

    }

    //public class CustomHttpMessageHandler : HttpClientHandler
    //{

    //    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

    //    public CustomHttpMessageHandler()
    //    {
    //        _retryPolicy = RetryPolicies.GetRetryPolicy();
    //    }

    //    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //    {
    //        // Perform custom logic before sending the request
    //        // For example, you can modify headers, add authentication tokens, etc.

    //        // Call the base SendAsync method to send the request
    //        var response = await base.SendAsync(request, cancellationToken);

    //        // Perform custom logic with the response
    //        // For example, you can handle errors, log responses, etc.

    //        return response;
    //    }
    //}

    //public class MyService : IBffApiClients
    //{
    //    private readonly IBffApiClients _api;

    //    public MyService()
    //    {
    //        //var httpClient = new HttpClient
    //        //{
    //        //    BaseAddress = new Uri("http://blazorwebapi.bff")
    //        //};

    //        //httpClient.BaseAddress = new Uri("http://blazorwebapi.bff");

    //        _api = RestService.For<IBffApiClients>();
    //    }

    //    private static readonly IReadOnlyCollection<HttpStatusCode> HttpStatusCodesWorthRetrying =
    //      new List<HttpStatusCode>
    //      {
    //            HttpStatusCode.RequestTimeout, // 408
    //            HttpStatusCode.InternalServerError, // 500
    //            HttpStatusCode.BadGateway, // 502
    //            HttpStatusCode.ServiceUnavailable, // 503
    //            HttpStatusCode.GatewayTimeout // 504
    //      };

    //    public static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy = Policy
    //        .Handle<HttpRequestException>()
    //        .OrResult<HttpResponseMessage>(r => HttpStatusCodesWorthRetrying.Contains(r.StatusCode))
    //        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2));

    //    public Task<HttpResponseMessage> GetAllRole()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public Task<HttpResponseMessage> GetRolesAsync()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<HttpResponseMessage> LoginAsync([Body] TokenRequest tokenRequest)
    //    {
    //        return await RetryPolicy.ExecuteAsync(async () => await _api.LoginAsync(tokenRequest));
    //    }

    //    public async Task<HttpResponseMessage> RefreshTokenAsync([Body] RefreshTokenRequest tokenRequest)
    //    {
    //        return await RetryPolicy.ExecuteAsync(async () => await _api.RefreshTokenAsync(tokenRequest));
    //    }

    //    public async Task<HttpResponseMessage> UserProfile([Header("Authorization")] string authorization)
    //    {
    //        return await RetryPolicy.ExecuteAsync(async () => await _api.UserProfile(authorization));
    //    }
    //}

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
