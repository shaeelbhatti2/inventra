using Inventra.Application.Abstractions;
using Inventra.Application.Auth;
using Inventra.Application.BatchLots;
using Inventra.Application.CycleCounts;
using Inventra.Application.Products;
using Inventra.Application.PurchaseOrders;
using Inventra.Application.SalesOrders;
using Inventra.Application.SerialNumbers;
using Inventra.Application.Transfers;
using Inventra.Application.Warehouses;
using Inventra.Application.StockLedger;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<StockLedgerService>();
        services.AddScoped<WarehouseService>();
        services.AddScoped<LocationService>();
        services.AddScoped<ProductService>();
        services.AddScoped<JwtTokenService>();
        services.AddSingleton<IAuthService, InMemoryAuthService>();
        services.AddScoped<PurchaseOrderService>();
        services.AddScoped<PurchaseOrderReceivingService>();
        services.AddScoped<SalesOrderService>();
        services.AddScoped<AllocationService>();
        services.AddScoped<TransferService>();
        services.AddScoped<CycleCountService>();
        services.AddScoped<BatchLotService>();
        services.AddScoped<SerialNumberService>();
        return services;
    }
}
