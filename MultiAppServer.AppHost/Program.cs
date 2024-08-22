using Aspire.Hosting;
using IdentityModel;

var builder = DistributedApplication.CreateBuilder(args);


//var sqlIndentity = builder.AddSqlServer("sql")
//                 .AddDatabase("db", "MultiAppServer.AppHost.Users");

//var redis = builder.AddRedis("redis");

//var rabbitMq = builder.AddRabbitMQ("eventbus");

//var user = builder.AddProject<Projects.BlazorIdentityUsers>("BlazorIdentityusers").WithReference(sqlIndentity).WithReference(messaging);

var launchProfileName = ShouldUseHttpForEndpoints() ? "http" : "https";

var user = builder.AddProject<Projects.BlazorIdentity>("blazorwebidentity", launchProfileName)
    .WithExternalHttpEndpoints();
//.WithReference(sqlIndentity)
//.WithReference(rabbitMq)
//.WithReference(redis);

var identityEndpoint = user.GetEndpoint(launchProfileName);
var file = builder.AddProject<Projects.BlazorApiUser>("blazoruser", launchProfileName);

user.WithEnvironment("IdentityUrl", identityEndpoint).WithEnvironment("CallBackUrl", user.GetEndpoint(launchProfileName)).WithEnvironment("IdentityApiClient", identityEndpoint).WithEnvironment("FileApiClient", file.GetEndpoint(launchProfileName));

file.WithEnvironment("Identity__Url", identityEndpoint).WithEnvironment("CallBackUrl", file.GetEndpoint(launchProfileName));

//builder.AddProject<Projects.WebApp>("webapp").WithReference(user);

var webApp = builder.AddProject<Projects.WebApp>("webapp", launchProfileName).WithEnvironment("IdentityUrl", identityEndpoint)
    //.WithReference(rabbitMq)
    //.WithReference(redis)
    .WithEnvironment("IdentityApiClient", identityEndpoint);

webApp.WithReference(user).WithReference(file).WithEnvironment("CallBackUrl", webApp.GetEndpoint(launchProfileName));

user.WithEnvironment("WebAppClient", webApp.GetEndpoint(launchProfileName));

builder.AddProject<Projects.BlazorApiUser>("blazorapiuser");

builder.Build().Run();

static bool ShouldUseHttpForEndpoints()
{
    const string EnvVarName = "ESHOP_USE_HTTP_ENDPOINTS";
    var envValue = Environment.GetEnvironmentVariable(EnvVarName);

    // Attempt to parse the environment variable value; return true if it's exactly "1".
    return int.TryParse(envValue, out int result) && result == 1;
}
