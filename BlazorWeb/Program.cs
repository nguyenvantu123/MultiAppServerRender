using Blazored.LocalStorage;
using BlazorWeb;
using BlazorWeb.Components;
using BlazorWeb.Components.Pages.Authentication;
using BlazorWeb.Constants.Application;
using BlazorWeb.Hubs;
using BlazorWeb.Identity;
using BlazorWeb.Manager.Preferences;
using BlazorWeb.Managers.Authentication;
using BlazorWeb.Managers.Preferences;
using BlazorWeb.Response.ResponseApiBase;
using BlazorWeb.Services.BffApiClients;
using BlazorWebApi.Users.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using MudBlazor;
using MudBlazor.Services;
using Polly;
using Refit;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Toolbelt.Blazor.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<AuthenticationHeaderHandler>();

var config = builder.Configuration;

var applicationSettingsConfiguration = config.GetSection(nameof(AppConfiguration));

builder.Services.Configure<AppConfiguration>(applicationSettingsConfiguration);

builder.Services.AddRefitClient<IBffApiClients>().ConfigureHttpClient(
    c => c.BaseAddress = new Uri("http://blazorwebapi.bff"))
    //.AddPolicyHandler((provider, _) => GetTokenRefresher(provider))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>();
//.AddPolicyHandler(
//    Policy<HttpResponseMessage>
//        .HandleResult(r => r.StatusCode == HttpStatusCode.Unauthorized
//         || r.StatusCode == HttpStatusCode.GatewayTimeout)
//        .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

builder.Services.AddTransient<IAuthenticationManager, AuthenManager>();
//builder.Services.AddScoped<IAuthenticationManager>();

builder.Services.AddSingleton<SignalRHub>();

builder.Services.AddScoped<AuthenticationStateProvider, UserStateProvider>();
builder.Services.AddTransient<IAuthenticationManager, AuthenManager>();

builder.Services.AddTransient<AuthenticationHeaderHandler>();

builder.Services.AddRefitClient<IBffApiClients>().ConfigureHttpClient(c => c.BaseAddress = new Uri("http://blazorwebapi.users"))
    .AddHttpMessageHandler<AuthenticationHeaderHandler>().AddPolicyHandler(
        Policy<HttpResponseMessage>
            .HandleResult(r => r.StatusCode == System.Net.HttpStatusCode.Unauthorized
             || r.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
             || r.StatusCode == HttpStatusCode.GatewayTimeout)
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

// Supply HttpClient instances that include access tokens when making requests to the server project
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("IBffApiClients"));

builder.Services.AddTransient<IResponseApiBase, ResponseApiBase>();

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

builder.Services.AddBlazoredLocalStorage().AddScoped<UserStateProvider>();


builder.Services.AddHttpClientInterceptor();

//builder.Services.AddControllers().AddJsonOptions(o =>
//{
//    o.JsonSerializerOptions.Converters.Add(new SystemTextJsonExceptionConverter());
//    o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//});

builder.Services.AddScoped<UserStateProvider>();

builder.Services.AddAuthorization();


var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

//IAsyncPolicy<HttpResponseMessage> GetTokenRefresher(IServiceProvider provider)
//{
//    return Policy<HttpResponseMessage>
//        .Handle<OutdatedTokenException>()
//        .RetryAsync(async (_, __) => await provider.GetRequiredService<UserStateProvider>().RefreshToken());
//}