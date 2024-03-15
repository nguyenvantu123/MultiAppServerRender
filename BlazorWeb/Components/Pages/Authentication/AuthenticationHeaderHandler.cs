using Blazored.LocalStorage;
using BlazorWeb.Constants.Application;
using BlazorWeb.Extensions;
using BlazorWeb.Hubs;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using BlazorWebApi.Users.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using Polly;
using Polly.Retry;
using Refit;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWeb.Components.Pages.Authentication
{
    //public interface ITokenManager
    //{
    //    Task<string> GetTokenAsync(string TokenKey);

    //    Task SetTokenAsync(string TokenKey, string token);

    //    Task RefreshTokenAsync();
    //}

    public class AuthenticationHeaderHandler : DelegatingHandler
    {

        //private readonly ITokenManager _tokenManager;
        //private readonly IJSRuntime _jsRuntime;

        public AuthenticationHeaderHandler(
            //ITokenManager tokenManager
            )
        {
            //_tokenManager = tokenManager;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            var response = await base.SendAsync(request, cancellationToken);

            //var result = await response.ToResult();
            return response;

        }
    }

    //public class TokenManager : ITokenManager
    //{
    //    private readonly ILocalStorageService _sessionStorage;
    //    private string TokenKey = StorageConstants.Local.AuthToken;
    //    private string RefreshTokenKey = StorageConstants.Local.RefreshToken;


    //    public TokenManager(ILocalStorageService sessionStorage)
    //    {
    //        _sessionStorage = sessionStorage;
    //    }

    //    public async Task<string> GetTokenAsync(string TokenKey)
    //    {
    //        return await _sessionStorage.GetItemAsync<string>(TokenKey);
    //    }

    //    public async Task SetTokenAsync(string TokenKey, string token)
    //    {
    //        await _sessionStorage.SetItemAsStringAsync(TokenKey, token);
    //    }

    //    public async Task RefreshTokenAsync()
    //    {
    //        // Implement token refresh logic here
    //        // Retrieve the current token from session state
    //        var currentToken = await GetTokenAsync(TokenKey);
    //        var currentRefreshToken = await GetTokenAsync(RefreshTokenKey);

    //        // Perform token refresh operation and update the token in session state
    //        var refreshedToken = "new_token_value"; // Example new token value
    //        await SetTokenAsync(TokenKey, refreshedToken);
    //    }
    //}
}