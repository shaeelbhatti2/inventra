using Inventra.Application;
using Inventra.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var host = builder.Build();
host.Run();
