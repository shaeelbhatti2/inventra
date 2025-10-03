using Inventra.Application;
using Inventra.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
