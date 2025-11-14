using Inventra.Application;
using Inventra.Infrastructure;
using Inventra.Jobs;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<AlertBackgroundWorker>();

var host = builder.Build();
host.Run();
