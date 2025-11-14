using Inventra.Application.Alerts;
using Inventra.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Inventra.Jobs;

public sealed class AlertBackgroundWorker : BackgroundService
{
    private readonly IServiceProvider _services;

    public AlertBackgroundWorker(IServiceProvider services) => _services = services;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var alerts = scope.ServiceProvider.GetRequiredService<AlertService>();
            await alerts.RunLowStockCheckAsync(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), stoppingToken);
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}
