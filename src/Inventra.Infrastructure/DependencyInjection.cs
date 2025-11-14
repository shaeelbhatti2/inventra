using Inventra.Application.Abstractions;
using Inventra.Infrastructure.Persistence;
using Inventra.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inventra.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? configuration["CONNECTION_STRING"]
            ?? "Host=localhost;Port=5433;Database=inventra;Username=inventra;Password=inventra_dev";

        services.AddDbContext<InventraDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IStockLedgerRepository, StockLedgerRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();
        services.AddScoped<ITransferOrderRepository, TransferOrderRepository>();
        services.AddScoped<ICycleCountRepository, CycleCountRepository>();
        services.AddScoped<IBatchLotRepository, BatchLotRepository>();
        services.AddScoped<ISerialNumberRepository, SerialNumberRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();

        return services;
    }
}
