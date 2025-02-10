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
using eShop.WebhookClient.Endpoints;
using WebApp.Shared;
using Syncfusion.Licensing;
using Syncfusion.Blazor;
using Syncfusion.Blazor.Popups;
using Aspire.StackExchange.Redis.DistributedCaching;
using Blazored.SessionStorage;
using IdentityModel;
using WebApp.Repositories;
var builder = WebApplication.CreateBuilder(args);


SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5ccnRTRmNeVkR0V0o=");
builder.AddServiceDefaults();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddSingleton<AppState>();
builder.Services.AddHttpClient<AccountApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("https://blazorapiuser");
}).AddApiVersion(1.0).AddAuthToken();

builder.Services.AddHttpClient<FileApiClient>(httpClient =>
{
    httpClient.BaseAddress = new("https://blazorfiles");
}).AddApiVersion(1.0).AddAuthToken();

builder.AddApplicationServices();

builder.AddRabbitMqEventBus("EventBus");

builder.AddRedisDistributedCache("Redis");

builder.Services.AddBlazoredSessionStorage();

builder.Services.AddSingleton<RedisUserRepository>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddMudServices();
//builder.Services.AddCascadingAuthenticationState();


//builder.Services.AddSingleton<CookieEvents>();
//builder.Services.AddScoped<AppState>();

var configuration = builder.Configuration;


//var identityUrl = configuration.GetRequiredValue("IdentityUrl");
//var callBackUrl = configuration.GetRequiredValue("CallBackUrl");
//var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

//string projectName = nameof(WebApp);

//builder.Services.AddSingleton<IAuthorizationPolicyProvider, SharedAuthorizationPolicyProvider>();
//builder.Services.AddTransient<IAuthorizationHandler, DomainRequirementHandler>();
//builder.Services.AddTransient<IAuthorizationHandler, EmailVerifiedHandler>();
//builder.Services.AddScoped<AuthenticationStateProvider, IdentityAuthenticationStateProvider>();

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();

var identitySection = configuration.GetSection("Identity");

var identityUrl = identitySection.GetRequiredValue("Url");
var callBackUrl = identitySection.GetRequiredValue("Url");

JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");
var sessionCookieLifetime = configuration.GetValue("SessionCookieLifetimeMinutes", 60);

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
}).AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime)).AddOpenIdConnect(options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = identityUrl;
    options.SignedOutRedirectUri = callBackUrl;
    options.ClientId = "webapp";
    options.ClientSecret = "secret";
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
    options.RequireHttpsMetadata = false;
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("files");
    options.Scope.Add("identity");
    options.Scope.Add("offline_access");
    options.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.Role, JwtClaimTypes.Role);
    options.MapInboundClaims = false;
});

//builder.Services.AddScoped<CustomAuthenticationStateProvider>();

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
//builder.Services.AddRazorPages();

//.AddOpenIdConnect( options =>
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

//builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();

//builder.Services.AddScoped(s =>
//{
//    // creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
//    var navigationManager = s.GetRequiredService<NavigationManager>();
//    var httpContextAccessor = s.GetRequiredService<IHttpContextAccessor>();
//    var cookies = httpContextAccessor.HttpContext.Request.Cookies;
//    var httpClientHandler = new HttpClientHandler() { UseCookies = false };
//    if (builder.Environment.IsDevelopment())
//    {
//        // Return 'true' to allow certificates that are untrusted/invalid
//        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
//    }
//    var client = new HttpClient(httpClientHandler);
//    if (cookies.Any())
//    {
//        var cks = new List<string>();

//        foreach (var cookie in cookies)
//        {
//            cks.Add($"{cookie.Key}={cookie.Value}");
//        }

//        client.DefaultRequestHeaders.Add("Cookie", string.Join(';', cks));
//    }

//    client.BaseAddress = new Uri(navigationManager.BaseUri);

//    return client;
//});

//builder.Services.AddScoped<IViewNotifier, ViewNotifier>();

//var authBuilder = builder.Services.AddAuthentication(options =>
//        {
//            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//            //options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//        }).AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(60));

//builder.Services.AddScoped<EntityPermissions>();

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


//builder.Services.AddHttpForwarderWithServiceDiscovery();


//builder.Services.AddRazorPages().AddMvcOptions(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});

//builder.Services.AddControllersWithViews(options =>
//         options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

#region Cookies
// cookie policy to deal with temporary browser incompatibilities
//builder.Services.AddSameSiteCookiePolicy();

//https://docs.microsoft.com/en-us/aspnet/core/security/gdpr
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    // This lambda determines whether user consent for non-essential
//    // cookies is needed for a given request.
//    options.CheckConsentNeeded = context => false; //consent not required
//                                                   // requires using Microsoft.AspNetCore.Http;
//                                                   //options.MinimumSameSitePolicy = SameSiteMode.None;
//});

#endregion

//builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseRequestLocalization(localizationOptions);

// Configure the HTTP request pipeline.
if (builder.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapAuthenticationEndpoints();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
