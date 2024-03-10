using Blazored.LocalStorage;
using BlazorWeb.Components.Pages.Authentication;
using BlazorWeb.Identity;
using BlazorWeb.Request.Identity;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using MultiAppServer.ServiceDefaults.Wrapper;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorWeb.Managers.Authentication
{
    public class AuthenManager : IAuthenticationManager
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IStringLocalizer<AuthenManager> _localizer;
        private readonly IBffApiClients _bffApiClients;

        public AuthenManager(
            ILocalStorageService localStorage,
            AuthenticationStateProvider authenticationStateProvider,
            IStringLocalizer<AuthenManager> localizer,
            IBffApiClients bffApiClients)
        {
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
            _localizer = localizer;
            _bffApiClients = bffApiClients;
        }

        public async Task<ClaimsPrincipal> CurrentUser()
        {
            var state = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return state.User;
        }

        public async Task<bool> Logout()
        {
            await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.ExpireIn);
            ((UserStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
            return true;
        }

        public async Task<string> RefreshToken()
        {
            var token = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            var refreshToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

            var response = await _bffApiClients.RefreshTokenAsync(new RefreshTokenRequest { accessToken = token, refreshToken = refreshToken });

            if (!response.Success)
            {
                return "";
            }

            token = response.Result.AccessToken;
            refreshToken = response.Result.RefreshToken;
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
            //await _localStorage.SetItemAsync(StorageConstants.Local.ExpireIn, refreshToken);

            return token;
        }

        public async Task<string> TryRefreshToken()
        {
            //check if token exists
            var availableToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);
            if (string.IsNullOrEmpty(availableToken)) return string.Empty;
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;
            var exp = user.FindFirst(c => c.Type.Equals("exp"))?.Value;
            var expTime = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(exp));
            var timeUTC = DateTime.UtcNow;
            var diff = expTime - timeUTC;
            if (diff.TotalMinutes <= 5)
                return await RefreshToken();
            return string.Empty;
        }

        public async Task<string> TryForceRefreshToken()
        {
            return await RefreshToken();
        }
    }
}