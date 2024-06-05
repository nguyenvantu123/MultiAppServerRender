using Aspire.RabbitMQ.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MultiAppServer.ServiceDefaults;
using System.Net;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.ConfigureJwtBearToken();

builder.AddRabbitMQ("messaging");

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
