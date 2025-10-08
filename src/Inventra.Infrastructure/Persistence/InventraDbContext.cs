using Inventra.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence;

public sealed class InventraDbContext : DbContext
{
    public InventraDbContext(DbContextOptions<InventraDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<StockLevel> StockLevels => Set<StockLevel>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderLine> PurchaseOrderLines => Set<PurchaseOrderLine>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<SalesOrderLine> SalesOrderLines => Set<SalesOrderLine>();
    public DbSet<TransferOrder> TransferOrders => Set<TransferOrder>();
    public DbSet<CycleCount> CycleCounts => Set<CycleCount>();
    public DbSet<CycleCountLine> CycleCountLines => Set<CycleCountLine>();
    public DbSet<BatchLot> BatchLots => Set<BatchLot>();
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();
    public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventraDbContext).Assembly);
    }
}
