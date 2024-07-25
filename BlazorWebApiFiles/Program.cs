using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Claims;
using System.Text;
using MultiAppServer.ServiceDefaults;
using BlazorWebApi.Files.Data;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

//builder.Services.AddTransient<IIntegrationEventLogService, IntegrationEventLogService<OrderingContext>>();

builder.AddRabbitMQ("Eventbus");

builder.AddSqlServerDbContext<FileDbContext>("FileDB");
builder.AddMinIoClient("MinioClient");

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IIdentityService, IdentityService>();

// Configure mediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining(typeof(Program));

    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidatorBehavior<,>));
    cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.ConfigureJwtBearToken();

//builder.AddRabbitMQ("messaging");

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var cs = builder.GetAppConfiguration();

if (cs.BehindSSLProxy)
{
    app.UseCors();
    app.UseForwardedHeaders();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();
