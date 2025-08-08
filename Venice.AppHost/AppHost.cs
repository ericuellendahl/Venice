var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Venice_Api>("venice-api");

builder.Build().Run();
