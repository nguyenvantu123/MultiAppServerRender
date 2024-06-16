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
using BlazorBoilerplate.Shared.Interfaces;
using BlazorBoilerplate.Shared.Services;

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
builder.Services.AddAuthorizationCore();

var authBuilder = builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
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

builder.Services.AddScoped<IAccountApiClient, AccountApiClient>();


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

//#region ExternalAuthProviders
////https://github.com/dotnet/aspfSignInSchemenetcore/blob/master/src/Security/Authentication/samples/SocialSample/Startup.cs
////https://docs.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins
//if (Convert.ToBoolean(configuration["ExternalAuthProviders:Google:Enabled"] ?? "false"))
//{
//    authBuilder.AddGoogle(options =>
//    {
//        options.ClientId = configuration["ExternalAuthProviders:Google:ClientId"];
//        options.ClientSecret = configuration["ExternalAuthProviders:Google:ClientSecret"];

//        options.AuthorizationEndpoint += "?prompt=consent"; // Hack so we always get a refresh token, it only comes on the first authorization response
//        options.AccessType = "offline";
//        options.SaveTokens = true;
//        options.Events = new OAuthEvents()
//        {
//            OnRemoteFailure = HandleOnRemoteFailure
//        };
//        options.ClaimActions.MapJsonSubKey("urn:google:image", "image", "url");
//        options.ClaimActions.Remove(ClaimTypes.GivenName);
//    });
//}

//if (Convert.ToBoolean(configuration["ExternalAuthProviders:Facebook:Enabled"] ?? "false"))
//{
//    // You must first create an app with Facebook and add its ID and Secret to your user-secrets.
//    // https://developers.facebook.com/apps/
//    // https://developers.facebook.com/docs/facebook-login/manually-build-a-login-flow#login
//    authBuilder.AddFacebook(options =>
//    {
//        options.AppId = Configuration["ExternalAuthProviders:Facebook:AppId"];
//        options.AppSecret = Configuration["ExternalAuthProviders:Facebook:AppSecret"];

//        options.Scope.Add("email");
//        options.Fields.Add("name");
//        options.Fields.Add("email");
//        options.SaveTokens = true;
//        options.Events = new OAuthEvents()
//        {
//            OnRemoteFailure = HandleOnRemoteFailure
//        };
//    });
//}

//if (Convert.ToBoolean(configuration["ExternalAuthProviders:Twitter:Enabled"] ?? "false"))
//{
//    // You must first create an app with Twitter and add its key and Secret to your user-secrets.
//    // https://apps.twitter.com/
//    // https://developer.twitter.com/en/docs/basics/authentication/api-reference/access_token
//    authBuilder.AddTwitter(options =>
//    {
//        options.ConsumerKey = Configuration["ExternalAuthProviders:Twitter:ConsumerKey"];
//        options.ConsumerSecret = Configuration["ExternalAuthProviders:Twitter:ConsumerSecret"];

//        // http://stackoverflow.com/questions/22627083/can-we-get-email-id-from-twitter-oauth-api/32852370#32852370
//        // http://stackoverflow.com/questions/36330675/get-users-email-from-twitter-api-for-external-login-authentication-asp-net-mvc?lq=1
//        options.RetrieveUserDetails = true;
//        options.SaveTokens = true;
//        options.ClaimActions.MapJsonKey("urn:twitter:profilepicture", "profile_image_url", ClaimTypes.Uri);
//        options.Events = new TwitterEvents()
//        {
//            OnRemoteFailure = HandleOnRemoteFailure
//        };
//    });
//}

////https://github.com/xamarin/Essentials/blob/master/Samples/Sample.Server.WebAuthenticator/Startup.cs
//if (Convert.ToBoolean(configuration["ExternalAuthProviders:Apple:Enabled"] ?? "false"))
//{
//    authBuilder.AddApple(options =>
//    {
//        options.ClientId = Configuration["ExternalAuthProviders:Apple:ClientId"];
//        options.KeyId = Configuration["ExternalAuthProviders:Apple:KeyId"];
//        options.TeamId = Configuration["ExternalAuthProviders:Apple:TeamId"];

//        options.UsePrivateKey(keyId
//           => _environment.ContentRootFileProvider.GetFileInfo($"AuthKey_{keyId}.p8"));
//        options.SaveTokens = true;
//    });
//}

//// You must first create an app with Microsoft Account and add its ID and Secret to your user-secrets.
//// https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app
//if (Convert.ToBoolean(configuration["ExternalAuthProviders:Microsoft:Enabled"] ?? "false"))
//{
//    authBuilder.AddMicrosoftAccount(options =>
//    {
//        options.ClientId = Configuration["ExternalAuthProviders:Microsoft:ClientId"];
//        options.ClientSecret = Configuration["ExternalAuthProviders:Microsoft:ClientSecret"];

//        options.SaveTokens = true;
//        options.Scope.Add("offline_access");
//        options.Events = new OAuthEvents()
//        {
//            OnRemoteFailure = HandleOnRemoteFailure
//        };
//    });
//}
//#endregion


//(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
//})
//.AddCookie(options => options.ExpireTimeSpan = TimeSpan.FromMinutes(sessionCookieLifetime))
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
//    options.Scope.Add("orders");
//    options.Scope.Add("basket");
//});


//builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

//var authConfigName = "OidcSettings";

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
//    //options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
//})
//.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
//.AddOpenIdConnect(options =>
//{
//    options.Authority = builder.Configuration[$"{authConfigName}:Authority"];
//    options.ClientId = builder.Configuration[$"{authConfigName}:ClientId"];
//    options.ClientSecret = builder.Configuration[$"{authConfigName}:ClientSecret"];
//    options.ResponseType = builder.Configuration[$"{authConfigName}:ResponseType"];
//    options.Scope.Add("APIAccess");

//    options.SaveTokens = true;

//    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

//    options.RequireHttpsMetadata = false;
//});


//builder.Services.AddFluentValidationAutoValidation();

//builder.Services.AddServerSideBlazor().AddCircuitOptions(o =>
//{
//    o.DetailedErrors = Convert.ToBoolean(Configuration[$"{projectName}:DetailedErrors"] ?? bool.FalseString);

//    if (_environment.IsDevelopment())
//    {
//        o.DetailedErrors = true;
//    }
//}).AddHubOptions(o =>
//{
//    o.MaximumReceiveMessageSize = 131072;
//});


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
