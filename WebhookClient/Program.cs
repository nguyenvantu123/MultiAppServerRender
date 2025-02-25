using MultiAppServer.ServiceDefaults;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
using WebhookClient.Components;

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();

builder.Services.AddRazorComponents().AddInteractiveServerComponents();
SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NDaF5cWWtCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdnWH5ccnRTRmNeVkR0V0o=");
builder.Services.AddServerSideBlazor();
//builder.Services.AddScoped<SfDialogService>();
builder.Services.AddDataProtection();
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
