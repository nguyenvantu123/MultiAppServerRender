using Duende.AccessTokenManagement.OpenIdConnect;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiAppServer.ServiceDefaults;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using WebhookClient.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5ccnRTRmNeVkR0V0o=");
//builder.Services.AddHttpContextAccessor();
builder.Services.AddLogging();
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(1);
});

builder.AddApplicationServices();
builder.Services.AddDataProtection();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
builder.Services.AddOpenIdConnectAccessTokenManagement()
    .AddBlazorServerAccessTokenManagement<ServerSideTokenStore>();

builder.Services.AddSingleton<IUserTokenStore, ServerSideTokenStore>();
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
