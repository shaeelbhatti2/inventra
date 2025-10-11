using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Abstractions;

public interface IStockLedgerRepository
{
    Task<StockLevel?> GetLevelAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct);
    Task AddMovementAsync(StockMovement movement, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
    Task<IReadOnlyList<StockMovement>> GetMovementsAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct);
}

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Warehouse>> ListAsync(Guid organizationId, CancellationToken ct);
    Task AddAsync(Warehouse warehouse, CancellationToken ct);
    Task<Location?> GetLocationByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<Location>> ListLocationsAsync(Guid organizationId, Guid warehouseId, CancellationToken ct);
    Task AddLocationAsync(Location location, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<ProductVariant?> GetVariantByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<ProductVariant?> GetVariantBySkuAsync(Guid organizationId, string sku, CancellationToken ct);
    Task<ProductVariant?> GetVariantByBarcodeAsync(Guid organizationId, string barcode, CancellationToken ct);
    Task<IReadOnlyList<Product>> ListAsync(Guid organizationId, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    Task AddVariantAsync(ProductVariant variant, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<PurchaseOrder>> ListAsync(Guid organizationId, CancellationToken ct);
    Task AddAsync(PurchaseOrder order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task<IReadOnlyList<SalesOrder>> ListAsync(Guid organizationId, CancellationToken ct);
    Task AddAsync(SalesOrder order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface ITransferOrderRepository
{
    Task<TransferOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task AddAsync(TransferOrder order, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface ICycleCountRepository
{
    Task<CycleCount?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct);
    Task AddAsync(CycleCount count, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IAuditRepository
{
    Task AddAsync(AuditLogEntry entry, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IBatchLotRepository
{
    Task<BatchLot?> GetByLotNumberAsync(Guid organizationId, Guid variantId, string lotNumber, CancellationToken ct);
    Task<IReadOnlyList<BatchLot>> ListByVariantAsync(Guid organizationId, Guid variantId, CancellationToken ct);
    Task AddAsync(BatchLot lot, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface ISerialNumberRepository
{
    Task<SerialNumber?> GetByNumberAsync(Guid organizationId, string number, CancellationToken ct);
    Task AddAsync(SerialNumber serial, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public interface IAlertRepository
{
    Task<bool> ExistsAsync(Guid organizationId, string alertKey, CancellationToken ct);
    Task AddAsync(StockAlert alert, CancellationToken ct);
    Task<IReadOnlyList<StockAlert>> ListOpenAsync(Guid organizationId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

public sealed class StockAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrganizationId { get; set; }
    public string AlertKey { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public bool IsRead { get; set; }
}
