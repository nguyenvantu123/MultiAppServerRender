using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Services;
using ServiceDefaults;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authorization;
using BlazorBoilerplate.Infrastructure.Storage.Permissions;
using BlazorBoilerplate.Server.Authorization;
using BlazorBoilerplate.Infrastructure.AuthorizationDefinitions;
using Microsoft.VisualBasic;
using WebApp.Settings;
using static BlazorBoilerplate.Constants.PasswordPolicy;
using static Microsoft.AspNetCore.Http.StatusCodes;
using BlazorBoilerplate.Server.Extensions;
using WebApp.Interfaces;
using BlazorBoilerplate.Theme.Material.Services;
using eShop.ServiceDefaults;
using Breeze.AspNetCore;
using Breeze.Core;
using Newtonsoft.Json.Serialization;
using WebApp.Localizer;
using FluentValidation.AspNetCore;
using Microsoft.JSInterop;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateRevalidation>();

builder.Services.AddSingleton<CookieEvents>();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.EventsType = typeof(CookieEvents);
});

var configuration = builder.Configuration;


//var identityUrl = configuration.GetRequiredValue("IdentityUrl");
//var callBackUrl = configuration.GetRequiredValue("CallBackUrl");
//var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

string projectName = nameof(WebApp);

//builder.Services.AddAuthorization();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IViewNotifier, ViewNotifier>();

var authBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        }).AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(60));

builder.Services.AddScoped<EntityPermissions>();
builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
builder.Services.AddTransient<IAuthorizationHandler, DomainRequirementHandler>();
builder.Services.AddTransient<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddTransient<IAuthorizationHandler, PermissionRequirementHandler>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();


builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = RequireDigit;
    options.Password.RequiredLength = RequiredLength;
    options.Password.RequireNonAlphanumeric = RequireNonAlphanumeric;
    options.Password.RequireUppercase = RequireUppercase;
    options.Password.RequireLowercase = RequireLowercase;
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
                   return factory.Create(typeof(Global));
               };
           });
//.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<LocalizationRecordValidator>());

builder.Services.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<IAccountApiClient, AccountApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("http://blazorwebapiusers");
}).AddAuthToken();


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

//services.ConfigureExternalCookie(options =>
// {
// macOS login fix
//options.Cookie.SameSite = SameSiteMode.None;
//});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.IsEssential = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromDays(Convert.ToDouble(configuration[$"{projectName}:CookieExpireTimeSpanDays"] ?? "30"));
    options.LoginPath = Settings.LoginPath;
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    // ReturnUrlParameter requires
    //using Microsoft.AspNetCore.Authentication.Cookies;
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;

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

builder.Services.AddCascadingAuthenticationState();

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

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();