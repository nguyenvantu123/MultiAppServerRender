using BlazorWebApi.Bff;
using BlazorWebApi.Bff.ApiClients;
using BlazorWebApi.Bff.Services.Identity;
using BlazorWebApi.Bff.Services.User;
using BlazorWebApi.Bff.Wrapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Refit;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using MultiAppServer.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Bff MultiAppServer.AppHost",
        Description = "MultiAppServer.AppHost",
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://github.com/ignaciojvig/ChatAPI/blob/master/LICENSE")
        }

    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
     {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
     });
    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    //c.IncludeXmlComments(xmlPath);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddRefitClient<IIdentityApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://blazorwebapi.users"))
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddRefitClient<IUserApiClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://blazorwebapi.users"))
                .AddHttpMessageHandler<AuthenticationHeaderHandler>();

builder.Services.AddTransient<AuthenticationHeaderHandler>();
builder.Services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

//builder.ConfigureJwtBearToken();

builder.Services.AddAuthorization();

var app = builder.Build();

app.MapDefaultEndpoints();
app.AddIdentityEndpoint();
app.AddUserEndpoint();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

var cs = builder.GetAppConfiguration();

if (cs.BehindSSLProxy)
{
    app.UseCors();
    app.UseForwardedHeaders();
}

app.UseAuthentication();

app.MapControllers();

app.Run();
