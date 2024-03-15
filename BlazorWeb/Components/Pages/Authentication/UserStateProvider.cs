using Blazored.LocalStorage;
using BlazorWeb.Constants.Permission;
using BlazorWeb.Extensions;
using BlazorWeb.Request.Identity;
using BlazorWeb.Response.Identity;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using BlazorWebApi.Users.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorWeb.Components.Pages.Authentication
{
    public class UserStateProvider : AuthenticationStateProvider
    {

        public ILocalStorageService _localStorage;
        //public HttpClient _httpClient;
        //private readonly IBffApiClients _bffApiClients;
        private readonly NavigationManager _navigationManager;

        //private readonly AppConfiguration _appConfig;

        public UserStateProvider(
            ILocalStorageService localStorage,
            //HttpClient httpClient,
            //IBffApiClients bffApiClients,
            NavigationManager navigationManager
            //, IOptions<AppConfiguration> appConfig
            )
        {
            _localStorage = localStorage;
            //_httpClient = httpClient;
            //_bffApiClients = bffApiClients;
            _navigationManager = navigationManager;

            //_appConfig = appConfig.Value;
        }

        public async Task StateChangedAsync()
        {
            var authState = Task.FromResult(await GetAuthenticationStateAsync());

            NotifyAuthenticationStateChanged(authState);

        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<ClaimsPrincipal> GetAuthenticationStateProviderUserAsync()
        {
            var state = await this.GetAuthenticationStateAsync();
            var authenticationStateProviderUser = state.User;
            return authenticationStateProviderUser;
        }

        public ClaimsPrincipal AuthenticationStateUser { get; set; }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {

            var accessToken = (await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken));

            if (string.IsNullOrEmpty(accessToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            var state = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(GetClaimsFromJwt(accessToken), "jwt")));
            AuthenticationStateUser = state.User;

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            return state;
        }

        public async Task<string> GetCurrentToken()
        {

            var accessToken = (await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken));

            return accessToken;
        }



        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
            await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);

            _navigationManager.NavigateTo("/login");

            MarkUserAsLoggedOut();

        }

        //public async Task<bool> RefreshToken()
        //{
        //    //var token = _appConfig.AccessToken;
        //    //var refreshToken = _appConfig.RefreshToken;

        //    var accessToken = (await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken));

        //    var refreshToken = (await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken));

        //    //var response = await _bffApiClients.RefreshTokenAsync(new RefreshTokenRequest { accessToken = accessToken, refreshToken = refreshToken });

        //    var result = await response.ToResult<TokenResponse>();

        //    if (!result.Success)
        //    {
        //        await Logout();

        //        return false;
        //    }

        //    await _localStorage.SetItemAsStringAsync(StorageConstants.Local.AuthToken, result.Result.AccessToken);
        //    await _localStorage.SetItemAsStringAsync(StorageConstants.Local.RefreshToken, result.Result.RefreshToken);

        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

        //    return true;
        //}

        public async Task<bool> Login(string accessToken, string refreshToken)
        {
            await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, accessToken);
            await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);

            //_appConfig.AccessToken = accessToken;
            //_appConfig.RefreshToken = refreshToken;

            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);

            await StateChangedAsync();

            return true;
        }

        private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            if (keyValuePairs != null)
            {
                keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

                if (roles != null)
                {
                    if (roles.ToString().Trim().StartsWith("["))
                    {
                        var parsedRoles = JsonSerializer.Deserialize<string[]>(roles.ToString());

                        claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, roles.ToString()));
                    }

                    keyValuePairs.Remove(ClaimTypes.Role);
                }

                keyValuePairs.TryGetValue(ApplicationClaimTypes.Permission, out var permissions);
                if (permissions != null)
                {
                    if (permissions.ToString().Trim().StartsWith("["))
                    {
                        var parsedPermissions = JsonSerializer.Deserialize<string[]>(permissions.ToString());
                        claims.AddRange(parsedPermissions.Select(permission => new Claim(ApplicationClaimTypes.Permission, permission)));
                    }
                    else
                    {
                        claims.Add(new Claim(ApplicationClaimTypes.Permission, permissions.ToString()));
                    }
                    keyValuePairs.Remove(ApplicationClaimTypes.Permission);
                }

                claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString())));
            }
            return claims;
        }

        private byte[] ParseBase64WithoutPadding(string payload)
        {
            payload = payload.Trim().Replace('-', '+').Replace('_', '/');
            var base64 = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
            return Convert.FromBase64String(base64);
        }
    }
}