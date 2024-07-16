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

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

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


//var identityUrl = configuration.GetRequiredValue("IdentityUrl");
//var callBackUrl = configuration.GetRequiredValue("CallBackUrl");

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(x =>
{
    x.LoginPath = WebApp.Settings.Settings.LoginPath;
    x.ExpireTimeSpan = TimeSpan.FromMinutes(60);
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped(s =>
{
    // creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
    var navigationManager = s.GetRequiredService<NavigationManager>();
    var httpContextAccessor = s.GetRequiredService<IHttpContextAccessor>();
    var cookies = httpContextAccessor.HttpContext.Request.Cookies;
    var httpClientHandler = new HttpClientHandler() { UseCookies = false };
    if (builder.Environment.IsDevelopment())
    {
        // Return 'true' to allow certificates that are untrusted/invalid
        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
    }
    var client = new HttpClient(httpClientHandler);
    if (cookies.Any())
    {
        var cks = new List<string>();

        foreach (var cookie in cookies)
        {
            cks.Add($"{cookie.Key}={cookie.Value}");
        }

        client.DefaultRequestHeaders.Add("Cookie", string.Join(';', cks));
    }

    client.BaseAddress = new Uri(navigationManager.BaseUri);

    return client;
});

builder.Services.AddScoped<IViewNotifier, ViewNotifier>();

//var authBuilder = builder.Services.AddAuthentication(options =>
//        {
//            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//            //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        }).AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(60));

builder.Services.AddScoped<EntityPermissions>();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    //options.Password.RequiredUniqueChars = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;

    if (Convert.ToBoolean(configuration[$"{projectName}:RequireConfirmedEmail"] ?? "false"))
    {
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    }
});

//builder.Services.AddHttpClient("MyHttpClient").SetHandlerLifetime(TimeSpan.FromHours(12));

builder.Services.AddMvc().AddNewtonsoftJson(opt =>
{
    // Set Breeze defaults for entity serialization
    var ss = JsonSerializationFns.UpdateWithDefaults(opt.SerializerSettings);
    if (ss.ContractResolver is DefaultContractResolver resolver)
    {
        resolver.NamingStrategy = null;  // remove json camelCasing; names are converted on the client.
    }
})   // Add Breeze exception filter to send errors back to the client
           .AddMvcOptions(o => { o.Filters.Add(new GlobalExceptionFilter()); })
           .AddViewLocalization().AddDataAnnotationsLocalization(options =>
           {
               options.DataAnnotationLocalizerProvider = (type, factory) =>
               {
                   return factory.Create(typeof(WebApp.Localizer.Global));
               };
           });
//.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LocalizationRecordValidator>());

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<AccountApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("http://blazorwebapiusers");
}).AddApiVersion(1.0).SetHandlerLifetime(TimeSpan.FromHours(12));


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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToDouble(configuration[$"{projectName}:CookieExpireTimeSpanDays"] ?? "60"));
    options.LoginPath = Settings.LoginPath;
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // ReturnUrlParameter requires
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;

    options.EventsType = typeof(CookieEvents);

    // Suppress redirect on API URLs in ASP.NET Core -> https://stackoverflow.com/a/56384729/54159
    options.Events = new CookieAuthenticationEvents()
    {
        OnRedirectToAccessDenied = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = Status403Forbidden;
            }

            return Task.CompletedTask;
        },
        OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = Status401Unauthorized;
            return Task.CompletedTask;
        }
    };
});
#endregion

//builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

app.MapDefaultEndpoints();

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
    .AddInteractiveServerRenderMode();

app.Run();
