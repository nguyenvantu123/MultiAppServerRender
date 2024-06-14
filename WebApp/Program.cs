using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using MudBlazor.Services;
using WebApp.Components;
using WebApp.Services;

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

var authConfigName = "OidcSettings";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(options =>
{
    options.Authority = builder.Configuration[$"{authConfigName}:Authority"];
    options.ClientId = builder.Configuration[$"{authConfigName}:ClientId"];
    options.ClientSecret = builder.Configuration[$"{authConfigName}:ClientSecret"];
    options.ResponseType = builder.Configuration[$"{authConfigName}:ResponseType"];
    options.Scope.Add("APIAccess");

    options.SaveTokens = true;

    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;

    options.RequireHttpsMetadata = false;
});


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
