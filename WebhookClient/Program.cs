using MultiAppServer.ServiceDefaults;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using WebhookClient.Components;

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
SyncfusionLicenseProvider.RegisterLicense("");
builder.Services.AddServerSideBlazor();
//builder.Services.AddScoped<SfDialogService>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddSyncfusionBlazor();
// Register the Syncfusion locale service to customize the  SyncfusionBlazor component locale culture
builder.Services.AddServerSideBlazor().AddCircuitOptions(option => { option.DetailedErrors = true; });
builder.Services.AddSignalR(o => { o.MaximumReceiveMessageSize = 102400000; });

builder.AddApplicationServices();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAntiforgery();

app.UseStaticFiles();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.MapAuthenticationEndpoints();

app.MapWebhookEndpoints();

app.Run();
