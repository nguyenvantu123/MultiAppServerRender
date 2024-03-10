var builder = DistributedApplication.CreateBuilder(args);

var sqlIndentity = builder.AddSqlServer("sql")
                 .AddDatabase("MultiAppServer.Users");

var user = builder.AddProject<Projects.BlazorWebApi_Users>("blazorwebapi.users").WithReference(sqlIndentity);

var bff = builder.AddProject<Projects.BlazorWebApi_Bff>("blazorwebapi.bff").WithReference(user);

builder.AddProject<Projects.BlazorWebApi_Files>("blazorwebapi.files");

builder.AddProject<Projects.BlazorWeb>("blazorweb").WithReference(user);

builder.Build().Run();
