using Inventra.Application.StockLedger;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<StockLedgerService>();
        return services;
    }
}
