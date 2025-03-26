using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using MultiAppServer.ServiceDefaults;
using BlazorIdentity.Files.Data;
using BlazorIdentityFiles.Application.Behaviors;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using IntegrationEventLogEF.Services;
using BlazorIdentity.Repository;
using BlazorIdentityFiles.SeedWork;
using BetkingLol.DataAccess.UnitOfWork;
using Aspire.Pomelo.EntityFrameworkCore.MySql;
using Aspire.Minio.Client;
using Minio;

var builder = WebApplication.CreateBuilder(args);


//builder.AddMySqlDataSource("FileDb");

builder.AddServiceDefaults();
builder.AddRabbitMqEventBus("EventBus");
builder.AddMySqlDbContext<FileDbContext>("FileDb");

//builder.AddSqlServerDbContext<FileDbContext>("FileDb");

builder.AddDefaultAuthentication();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IFilesQueries, FilesQueries>();

builder.Services.AddScoped<IRequestManager, RequestManager> ();

builder.Services.AddScoped<FileServices>();

var configSection = builder.Configuration.GetSection("MinioClient");

var settings = new MinIoClientSettings();
configSection.Bind(settings);

builder.Services.AddMinio(configureClient => configureClient
       .WithEndpoint(settings.Endpoint)
       .WithSSL(true)
       .WithCredentials(settings.AccessKey, settings.SecretKey));

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<FileDbContext>>();
builder.Services.AddAntiforgery();

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
app.UseAntiforgery();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

var files = app.NewVersionedApi("Files");

files.MapFilesApiV1()
      .RequireAuthorization();

app.UseDefaultOpenApi();

app.Run();
