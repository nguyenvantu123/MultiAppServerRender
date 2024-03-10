using Blazored.LocalStorage;
using BlazorWeb.Constants.Application;
using BlazorWeb.Extensions;
using BlazorWeb.Hubs;
using BlazorWeb.Identity;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
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
        private readonly SignalRHub _signalRHub;
        //private ILocalStorageService _localStorageService;

        public AuthenticationHeaderHandler(
           SignalRHub signalRHub
           )
        {
            _signalRHub = signalRHub;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            var data = await response.ToResult();

            if (data.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                await _signalRHub.RegenerateTokensAsync();
            }
            return response;

        }
    }

}