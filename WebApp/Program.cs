using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Components;
using MultiAppServer.ServiceDefaults;
using Microsoft.AspNetCore.Components.Server;
using WebApp.Interfaces;
using FluentValidation.AspNetCore;
using WebApp;
using WebApp.Extensions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authentication;
using WebApp.Shared;
using Syncfusion.Licensing;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Aspire.StackExchange.Redis.DistributedCaching;
using Blazored.SessionStorage;
using IdentityModel;
using Microsoft.AspNetCore.Http.Connections;
using WebApp.Repositories;
using WebApp.Endpoints;
using WebApp.Models;
var builder = WebApplication.CreateBuilder(args);


SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5ccnRTRmNeVkR0V0o=");
builder.AddServiceDefaults();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSingleton<AppState>();

//builder.Services.AddScoped<TokenProvider>();
var configuration = builder.Configuration;

var url = configuration.GetSection("HostUrl");

builder.Services.AddHttpClient<AccountApiClient>(httpClient =>
{
    httpClient.BaseAddress = new(url.GetRequiredValue("UserApi"));
}).AddApiVersion(1.0).AddAuthToken();

builder.Services.AddHttpClient<FileApiClient>(httpClient =>
{
    httpClient.BaseAddress = new(url.GetRequiredValue("FileApi"));
}).AddApiVersion(1.0).AddAuthToken();

builder.AddApplicationServices();
builder.AddRabbitMqEventBus("EventBus");

builder.AddRedisDistributedCache("Redis");

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddSingleton<RedisUserRepository>();

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddAuthorization();

var identitySection = configuration.GetSection("Identity");

var identityUrl = identitySection.GetRequiredValue("Url");
var callBackUrl = identitySection.GetRequiredValue("Url");

//JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

//builder.Services.AddControllersWithViews(options =>
//{
//    options.EnableEndpointRouting = false;
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
.AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = identityUrl;
    //options.SignedOutRedirectUri = callBackUrl;
    options.ClientId = "webapp";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
    options.MapInboundClaims = false;
    options.Scope.Add("webhooks");
    options.Scope.Add("users");
    options.Scope.Add("profile");
    options.Scope.Add("openid");
    options.Scope.Add("identity");
    options.Scope.Add("files");
});

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<SfDialogService>();
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSyncfusionBlazor();
// Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
builder.Services.AddSingleton(typeof(ISyncfusionStringLocalizer), typeof(SyncfusionLocalizer));

var supportedCultures = new[] { "en-US", "de-DE", "fr-CH", "zh-CN" };
var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture("en-US")
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });

builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseAntiforgery();

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//app.UseDeveloperExceptionPage();
//app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapAuthenticationEndpoints();

app.Run();
