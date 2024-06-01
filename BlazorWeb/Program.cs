using Blazored.LocalStorage;
using BlazorWeb;
using BlazorWeb.Components;
using BlazorWeb.Components.Pages.Authentication;
using BlazorWeb.Constants.Application;
using BlazorWeb.Hubs;
using BlazorWeb.Manager.Preferences;
using BlazorWeb.Managers.Preferences;
using BlazorWeb.Services;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Users.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using MudBlazor.Services;
using MultiAppServer.ServiceDefaults;
using Polly;
using Refit;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using static BlazorWeb.Components.Pages.Authentication.AuthenticationHeaderHandler;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddSingleton<LoadingService>();
builder.Services.AddScoped<ILoadingService, LoadingService>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.AddScoped<ITokenManager, TokenManager>();

//builder.Services.Addpre(options => options.Headers.Add("Authorization"));

//builder.Services.AddSingleton<MyService>();

//builder.Services.AddHttpClient("GlobalHttpClient", client =>
//{
//    client.BaseAddress = new Uri("https://api.example.com/");
//    // Add other HttpClient configurations here
//}).AddPolicyHandler(GetRetryPolicy());

//builder.Services.AddRefitClient<IBffApiClients>()
//          .ConfigureHttpClient((serviceProvider, client) =>
//          {
//              client.BaseAddress = new Uri("http://blazorwebapi.bff");
//          })
//          .ConfigurePrimaryHttpMessageHandler<HttpClientHandler>((serviceProvider, handler) =>
//          {
//              return serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("GlobalHttpClient");
//          });

//builder.Services.AddTransient(sp =>
//{
//    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
//    var httpClient = httpClientFactory.CreateClient("MyHttpClient");
//    return RestService.For<IBffApiClients>(httpClient);
//});

builder.Services.AddRefitClient<IBffApiClients>()
     .ConfigureHttpClient((client) =>
     {
         client.BaseAddress = new Uri("http://blazorwebapi.bff");
     })
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddTransient<AuthenticationHeaderHandler>();

var config = builder.Configuration;

var applicationSettingsConfiguration = config.GetSection(nameof(AppConfiguration));

builder.Services.Configure<AppConfiguration>(applicationSettingsConfiguration);

builder.Services.AddSingleton<SignalRHub>();

builder.Services.AddScoped<UserStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider, UserStateProvider>();

builder.Services.AddMudServices();
builder.Services.AddTransient<IClientPreferenceManager, ClientPreferenceManager>();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOptions();

builder.Services.AddMudServices(configuration =>
{
    configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    configuration.SnackbarConfiguration.HideTransitionDuration = 100;
    configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
    configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
    configuration.SnackbarConfiguration.ShowCloseIcon = false;
});

builder.ConfigureJwtBearToken();

builder.Services.AddAuthentication();

builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, BlazorAuthorizationMiddlewareResultHandler>();
builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
})
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddScoped<ClientPreferenceManager>();

builder.Services.AddBlazoredLocalStorage();


builder.Services.AddHttpClientInterceptor();

builder.Services.AddAuthorization();


var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

var cs = builder.GetAppConfiguration();

if (cs.BehindSSLProxy)
{
    app.UseCors();
    app.UseForwardedHeaders();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapHub<SignalRHub>(ApplicationConstants.SignalR.HubUrl);

app.UseAuthorization();
app.UseAuthentication();
app.Run();

//IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
//{
//    return Policy.Handle<HttpRequestException>()
//                 .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
//                 .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
//}