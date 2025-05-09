using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MultiAppServer.ServiceDefaults;
using Shared;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using WebhookClient;
using WebhookClient.Components;
using static Shared.HttpClientExtensions;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF1cX2hIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEBjXH9WcHZQR2FbWUZxWUlfZA==");
//builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
});

//builder.Services.AddMemoryCache();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
builder.Services.AddDataProtection();

builder.AddApplicationServices();
builder.Services.AddDataProtection();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
builder.Services
           .AddOpenIdConnectAccessTokenManagement()
           .AddBlazorServerAccessTokenManagement<ServerSideTokenStore>()
           .AddScoped<AuthenticationStateProvider, BffServerAuthenticationStateProvider>();

builder.Services.AddSingleton<IPostConfigureOptions<CookieAuthenticationOptions>, PostConfigureApplicationCookieTicketStore>();
builder.Services.AddTransient<IServerTicketStore, ServerSideTicketStore>();
builder.Services.AddTransient<ISessionRevocationService, SessionRevocationService>();
builder.Services.AddSingleton<IHostedService, SessionCleanupHost>();
builder.Services.AddScoped<WebhooksClient>();

// only add if not already in DI
builder.Services.TryAddSingleton<IUserSessionStore, InMemoryUserSessionStore>();

var configuration = builder.Configuration;

var url = configuration.GetSection("HostUrl");

builder.Services.AddUserAccessTokenHttpClient("callApi",
    configureClient: client => client.BaseAddress = new Uri(url.GetRequiredValue("WebhooksApi"))).AddApiVersion(1).AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>();

//builder.Services.TryAddTransient<HttpClientAuthorizationDelegatingHandler>();
//builder.Services.AddSingleton<IUserTokenStore, ServerSideTokenStore>();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseStaticFiles();
//app.UseAuthentication();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
//app.MapBlazorHub(options => options.Transports = HttpTransportType.WebSockets);
app.MapAuthenticationEndpoints();

app.MapWebhookEndpoints();

app.Run();


//var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();

//builder.Services.AddRazorComponents().AddInteractiveServerComponents();
//SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5ccnRTRmNeVkR0V0o=");
//builder.Services.AddServerSideBlazor();
////builder.Services.AddScoped<SfDialogService>();
//builder.Services.AddDataProtection();
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.AddSyncfusionBlazor();
//// Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
//builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });

//builder.AddApplicationServices();

//var app = builder.Build();

//app.MapDefaultEndpoints();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

//app.UseAntiforgery();

//app.UseStaticFiles();

//app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

//app.MapAuthenticationEndpoints();

//app.MapWebhookEndpoints();

//app.Run();
