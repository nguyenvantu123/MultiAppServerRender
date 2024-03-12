using Blazored.LocalStorage;
using BlazorWeb.Constants.Application;
using BlazorWeb.Extensions;
using BlazorWeb.Hubs;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using BlazorWebApi.Users.Configurations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using Refit;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BlazorWeb.Components.Pages.Authentication
{
    public class AuthenticationHeaderHandler : DelegatingHandler
    {
        //private ILocalStorageService _localStorageService;

        //private IBffApiClients _bffApiClients { get; set; }
        //private AppConfiguration _appConfig;


        //public AuthenticationHeaderHandler(IBffApiClients bffApiClients, IOptions<AppConfiguration> appConfig)
        //{
        //    _bffApiClients = bffApiClients;
        //    _appConfig = appConfig.Value;
        //}

        public AuthenticationHeaderHandler()
        {
                
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var data = await response.ToResult();

            if (data.StatusCode == (int)HttpStatusCode.Unauthorized)
            {


                //var newToken = await _bffApiClients.RefreshTokenAsync(new
                //    Request.Identity.RefreshTokenRequest
                //{
                //    accessToken = _appConfig.AccessToken,
                //    refreshToken = _appConfig.RefreshToken,
                //});

                //if (newToken.Success == true)
                //{
                //    _appConfig.AccessToken = newToken.Result.AccessToken;
                //    _appConfig.RefreshToken = newToken.Result.RefreshToken;

                //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _appConfig.AccessToken);

                //    response = await base.SendAsync(request, cancellationToken);
                //}
            }
            return response;

        }
    }

}