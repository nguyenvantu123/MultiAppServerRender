using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using MultiAppServer.ServiceDefaults;
using BlazorWebApi.Files.Data;
using BlazorWebApiFiles.Application.Behaviors;
using Minio;
using Aspire.Minio.Client;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using IntegrationEventLogEF.Services;
using BlazorWebApi.Repository;
using BlazorWebApiFiles.SeedWork;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();


builder.AddRabbitMQ("Eventbus");

builder.AddSqlServerDbContext<FileDbContext>("FileDb");

//builder.Services.AddSingleton<FileServices>();

builder.AddDefaultAuthentication();

builder.Services.AddScoped<IFilesQueries, FilesQueries>();

builder.Services.AddScoped<IRequestManager, RequestManager>();

var configSection = builder.Configuration.GetSection("MinioClient");

var settings = new MinIoClientSettings();
configSection.Bind(settings);

builder.Services.AddMinio(configureClient => configureClient
       .WithEndpoint(settings.Endpoint)
       .WithCredentials(settings.AccessKey, settings.SecretKey));

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<FileDbContext>>();

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});


var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);


var app = builder.Build();

app.UseAuthentication();


app.UseRouting();
app.UseAuthorization();
app.MapDefaultEndpoints();

var files = app.NewVersionedApi("Files");

files.MapFilesApiV1()
      .RequireAuthorization();

app.UseDefaultOpenApi();

app.Run();
