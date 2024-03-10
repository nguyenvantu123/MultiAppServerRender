using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Toolbelt.Blazor;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http;
using BlazorWeb.Request.Identity;
using Blazored.LocalStorage;

namespace BlazorWeb.Managers.Interceptors
{
    public class HttpInterceptorManager : IHttpInterceptorManager
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly IBffApiClients _bffApiClients;
        private readonly NavigationManager _navigationManager;
        private readonly ISnackbar _snackBar;
        private readonly IStringLocalizer<HttpInterceptorManager> _localizer;
        private readonly ILocalStorageService _localStorage;

        public HttpInterceptorManager(
            HttpClientInterceptor interceptor,
            IBffApiClients bffApiClients,
            NavigationManager navigationManager,
            ISnackbar snackBar,
            IStringLocalizer<HttpInterceptorManager> localizer,
            ILocalStorageService localStorage)
        {
            _interceptor = interceptor;
            _bffApiClients = bffApiClients;
            _navigationManager = navigationManager;
            _snackBar = snackBar;
            _localizer = localizer;
            _localStorage = localStorage;
        }

        public void RegisterEvent() => _interceptor.BeforeSendAsync += InterceptBeforeHttpAsync;

        public async Task InterceptBeforeHttpAsync(object sender, HttpClientInterceptorEventArgs e)
        {
            var absPath = e.Request.RequestUri.AbsolutePath;
            if (!absPath.Contains("token") && !absPath.Contains("accounts"))
            {
                try
                {
                    //var token = await _bffApiClients.RefreshTokenAsync(new RefreshTokenRequest { accessToken = });
                    //if (!string.IsNullOrEmpty(token))
                    //{
                    //    _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
                    //    e.Request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
                    //((UserStateProvider)pro).MarkUserAsLoggedOut();
                    //_httpClient.DefaultRequestHeaders.Authorization = null;
                    _navigationManager.NavigateTo("/");
                }
            }
        }

        public void DisposeEvent() => _interceptor.BeforeSendAsync -= InterceptBeforeHttpAsync;
    }
}