using BlazorIdentityApi.Users.Constants;
using BlazorIdentityApi.Repositories.Interfaces;
using BlazorIdentityApi.Resources;
using BlazorIdentityApi.Services.Interfaces;
using BlazorIdentityApi.Services;
using MultiAppServer.ServiceDefaults;
using BlazorIdentityApi.Repositories;
using BlazorIdentity.Data;
using BlazorIdentity.Users.Data;
using Aspire.Microsoft.EntityFrameworkCore.SqlServer;
using BlazorIdentity.Users.Models;
using Finbuckle.MultiTenant;
using BlazorIdentityApi.Mappers.Configuration;
using AutoMapper;
using BlazorIdentityApi.Mappers;
using Aspire.Pomelo.EntityFrameworkCore.MySql;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddApplicationServices();

var withApiVersioning = builder.Services.AddApiVersioning();

builder.AddDefaultOpenApi(withApiVersioning);
// Add services to the container.

//builder.AddSqlServerDbContext<ApplicationDbContext>("Identitydb");

//builder.AddSqlServerDbContext<TenantStoreDbContext>("Identitydb");

//builder.AddMySqlDataSource("Identitydb");

builder.AddMySqlDbContext<ApplicationDbContext>("Identitydb");
builder.AddMySqlDbContext<TenantStoreDbContext>("Identitydb");

builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHostStrategy("__tenant__")
    .WithEFCoreStore<TenantStoreDbContext, AppTenantInfo>()
    .WithStaticStrategy(DefaultTenant.DefaultTenantId);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IApiErrorResources, ApiErrorResources>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationConsts.AdministrationPolicy,
        policy =>
            policy.RequireAssertion(context => context.User.HasClaim(c => c.Value == "Administrator")
    ));
});

var profileTypes = new HashSet<Type>
            {
                typeof(IdentityMapperProfile)
            };

builder.Services.AddAdminAspNetIdentityMapping()
            .UseIdentityMappingProfile()
            .AddProfilesType(profileTypes);

builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddTransient<IIdentityResourceRepository, IdentityResourceRepository>();
builder.Services.AddTransient<IApiResourceRepository, ApiResourceRepository>();
builder.Services.AddTransient<IApiScopeRepository, ApiScopeRepository>();
builder.Services.AddTransient<IPersistedGrantRepository, PersistedGrantRepository>();
builder.Services.AddTransient<IIdentityProviderRepository, IdentityProviderRepository>();
builder.Services.AddTransient<IKeyRepository, KeyRepository>();
builder.Services.AddTransient<IDashboardRepository, DashboardRepository>();

builder.Services.AddTransient<IClientService, ClientService>();
builder.Services.AddTransient<IApiResourceService, ApiResourceService>();
builder.Services.AddTransient<IApiScopeService, ApiScopeService>();
builder.Services.AddTransient<IIdentityResourceService, IdentityResourceService>();
builder.Services.AddTransient<IIdentityProviderService, IdentityProviderService>();
builder.Services.AddTransient<IPersistedGrantService, PersistedGrantService>();
builder.Services.AddTransient<IKeyService, KeyService>();
builder.Services.AddTransient<IDashboardService, DashboardService>();

builder.Services.AddScoped<IApiResourceServiceResources, ApiResourceServiceResources>();
builder.Services.AddScoped<IApiScopeServiceResources, ApiScopeServiceResources>();
builder.Services.AddScoped<IClientServiceResources, ClientServiceResources>();
builder.Services.AddScoped<IIdentityResourceServiceResources, IdentityResourceServiceResources>();
builder.Services.AddScoped<IIdentityProviderServiceResources, IdentityProviderServiceResources>();
builder.Services.AddScoped<IPersistedGrantServiceResources, PersistedGrantServiceResources>();
builder.Services.AddScoped<IKeyServiceResources, KeyServiceResources>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseDefaultOpenApi();

app.Run();
