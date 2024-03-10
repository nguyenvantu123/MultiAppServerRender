using Blazored.LocalStorage;
using BlazorWeb.Constants.Application;
using BlazorWeb.Identity;
using BlazorWeb.Managers.Authentication;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Constants.Storage;
using MultiAppServer.ServiceDefaults.Wrapper;
using Newtonsoft.Json;
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


        public AuthenticationHeaderHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {

            var response = await base.SendAsync(request, cancellationToken);

            var data = JsonConvert.DeserializeObject<ResultBase<bool>>(await response.Content.ReadAsStringAsync());

            return response;
        }
    }
}