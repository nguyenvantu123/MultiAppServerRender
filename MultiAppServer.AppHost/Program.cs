var builder = DistributedApplication.CreateBuilder(args);


var sqlIndentity = builder.AddSqlServer("sql")
                 .AddDatabase("db", "MultiAppServer.Users");

var messaging = builder.AddRabbitMQ("messaging");

var fileCache = builder.AddRedis("fileCache");

// builder.AddProject<Projects.webfrontend>("webfrontend");

//builder.AddProject<Projects.BlazorWebApiBff>("blazorwebapibff");

//builder.AddProject<Projects.BlazorWebApiFiles>("blazorwebapifiles");

//builder.AddProject<Projects.BlazorWebApiUsers>("blazorwebapiusers");

var user = builder.AddProject<Projects.BlazorWebApiUsers>("blazorwebapiusers").WithReference(sqlIndentity).WithReference(messaging);

var bff = builder.AddProject<Projects.BlazorWebApiBff>("blazorwebapibff").WithReference(user).WithReference(messaging);

var file = builder.AddProject<Projects.BlazorWebApiFiles>("blazorwebapifiles").WithReference(messaging);

builder.AddProject<Projects.webfrontend>("webfrontend").WithReference(user).WithReference(bff);

builder.Build().Run();
