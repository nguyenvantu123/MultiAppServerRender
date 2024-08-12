using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Services;
using MultiAppServer.ServiceDefaults;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authorization;
using Microsoft.VisualBasic;
using WebApp.Settings;
using static Microsoft.AspNetCore.Http.StatusCodes;
using WebApp.Interfaces;
using Breeze.AspNetCore;
using Breeze.Core;
using Newtonsoft.Json.Serialization;
using WebApp.Localizer;
using FluentValidation.AspNetCore;
using Microsoft.JSInterop;
using System.Net.Http;
using WebApp;
using WebApp.Extensions;
using WebApp.Permissions;
using Microsoft.AspNetCore.Components;
using WebApp.State;
using BlazorWebApi.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using BlazorWebApi.Users.Models;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddApplicationServices();

builder.AddRabbitMQ("EventBus");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddCascadingAuthenticationState();


builder.Services.AddSingleton<CookieEvents>();
builder.Services.AddScoped<AppState>();

var configuration = builder.Configuration;


//var identityUrl = configuration.GetRequiredValue("IdentityUrl");
//var callBackUrl = configuration.GetRequiredValue("CallBackUrl");
//var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

string projectName = nameof(WebApp);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, SharedAuthorizationPolicyProvider>();
builder.Services.AddTransient<IAuthorizationHandler, DomainRequirementHandler>();
builder.Services.AddTransient<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

builder.Services.AddAuthorization();


var identityUrl = configuration.GetRequiredValue("IdentityUrl");
var callBackUrl = configuration.GetRequiredValue("CallBackUrl");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(x =>
{
    x.LoginPath = WebApp.Settings.Settings.LoginPath;
    x.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});
//.AddOpenIdConnect(options =>
//{
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.Authority = identityUrl;
//    options.SignedOutRedirectUri = callBackUrl;
//    options.ClientId = "webapp";
//    options.ClientSecret = "secret";
//    options.ResponseType = "code";
//    options.SaveTokens = true;
//    options.GetClaimsFromUserInfoEndpoint = true;
//    options.RequireHttpsMetadata = false;
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.Scope.Add("files");
//}); ;

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.AddScoped(s =>
//{

//    //x.LoginPath = WebApp.Settings.Settings.LoginPath;
//    //x.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.Authority = identityUrl;
//    options.SignedOutRedirectUri = callBackUrl;
//    options.ClientId = "webapp";
//    options.ClientSecret = "secret";
//    options.ResponseType = "code";
//    options.SaveTokens = true;
//    options.GetClaimsFromUserInfoEndpoint = true;
//    options.RequireHttpsMetadata = false;
//    options.Scope.Add("openid");
//    options.Scope.Add("profile");
//    options.Scope.Add("files");
//    options.Scope.Add("identity");
//});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddRazorPages().AddMvcOptions(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddScoped<IViewNotifier, ViewNotifier>();

builder.Services.AddScoped<EntityPermissions>();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequiredLength = 6;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequireLowercase = false;
//    //options.Password.RequiredUniqueChars = 6;

//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
//    options.Lockout.MaxFailedAccessAttempts = 10;
//    options.Lockout.AllowedForNewUsers = true;

//    if (Convert.ToBoolean(configuration[$"{projectName}:RequireConfirmedEmail"] ?? "false"))
//    {
//        options.User.RequireUniqueEmail = true;
//        options.SignIn.RequireConfirmedEmail = true;
//    }
//});

builder.Services.AddControllersWithViews(options =>
      options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

//builder.Services.AddHttpClient("MyHttpClient").SetHandlerLifetime(TimeSpan.FromHours(12));

//builder.Services.AddMvc().AddNewtonsoftJson(opt =>
//{
//    // Set Breeze defaults for entity serialization
//    var ss = JsonSerializationFns.UpdateWithDefaults(opt.SerializerSettings);
//    if (ss.ContractResolver is DefaultContractResolver resolver)
//    {
//        resolver.NamingStrategy = null;  // remove json camelCasing; names are converted on the client.
//    }
//})   // Add Breeze exception filter to send errors back to the client
//           .AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter()); })
//           .AddViewLocalization().AddDataAnnotationsLocalization(options =>
//           {
//               options.DataAnnotationLocalizerProvider = (type, factory) =>
//               {
//                   return factory.Create(typeof(WebApp.Localizer.Global));
//               };
//           });
//.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LocalizationRecordValidator>());

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<AccountApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("https://blazorwebapiusers");
}).AddApiVersion(1.0).SetHandlerLifetime(TimeSpan.FromHours(12)).AddAuthToken();

builder.Services.AddHttpClient<FileApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("https://blazorwebapifiles");
}).AddApiVersion(1.0).SetHandlerLifetime(TimeSpan.FromHours(12)).AddAuthToken();


#region Cookies
// cookie policy to deal with temporary browser incompatibilities
builder.Services.AddSameSiteCookiePolicy();

//https://docs.microsoft.com/en-us/aspnet/core/security/gdpr
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential
    // cookies is needed for a given request.
    options.CheckConsentNeeded = context => false; //consent not required
                                                   // requires using Microsoft.AspNetCore.Http;
                                                   //options.MinimumSameSitePolicy = SameSiteMode.None;
});

#endregion

//builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode().RequireAuthorization();

app.Run();
