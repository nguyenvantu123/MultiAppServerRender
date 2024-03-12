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

        public AppConfiguration Configuration { get; set; }

        public AuthenticationHeaderHandler(IOptions<AppConfiguration> AppConfiguration)
        {
            Configuration = AppConfiguration.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Configuration.AccessToken);

            var response = await base.SendAsync(request, cancellationToken);

            var stringData = await response.Content.ReadAsStringAsync();

            var data = await response.ToResult();

            if (data.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                throw new OutdatedTokenException();

            }
            return response;

        }
    }

}