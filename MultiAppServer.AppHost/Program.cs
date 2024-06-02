var builder = DistributedApplication.CreateBuilder(args);


var sqlIndentity = builder.AddSqlServer("sql")
                 .AddDatabase("db", "MultiAppServer.Users");

var messaging = builder.AddRabbitMQ("messaging");

var fileCache = builder.AddRedis("fileCache");

var user = builder.AddProject<Projects.BlazorWebApi_Users>("users-api").WithReference(sqlIndentity).WithReference(messaging);

var bff = builder.AddProject<Projects.BlazorWebApi_Bff>("bff").WithReference(user).WithReference(messaging);

var file = builder.AddProject<Projects.BlazorWebApi_Files>("files-api").WithReference(messaging);

builder.AddProject<Projects.BlazorWeb>("blazorweb").WithReference(user).WithReference(bff);
builder.Build().Run();
