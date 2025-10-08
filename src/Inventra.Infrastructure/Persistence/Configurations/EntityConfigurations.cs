using Inventra.Domain.Entities;
using Inventra.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventra.Infrastructure.Persistence.Configurations;

public sealed class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("organizations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.BaseCurrency).HasMaxLength(3).IsRequired();
        builder.Property(x => x.OverReceiptTolerancePercent).HasPrecision(5, 2);
    }
}

public sealed class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("warehouses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.TimeZoneId).HasMaxLength(64).IsRequired();
        builder.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.Line1).HasColumnName("address_line1").HasMaxLength(200);
            a.Property(p => p.Line2).HasColumnName("address_line2").HasMaxLength(200);
            a.Property(p => p.City).HasColumnName("address_city").HasMaxLength(100);
            a.Property(p => p.State).HasColumnName("address_state").HasMaxLength(100);
            a.Property(p => p.PostalCode).HasColumnName("address_postal_code").HasMaxLength(20);
            a.Property(p => p.Country).HasColumnName("address_country").HasMaxLength(100);
        });
        builder.HasIndex(x => new { x.OrganizationId, x.Code }).IsUnique();
    }
}

public sealed class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Code).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Barcode).HasMaxLength(64);
        builder.HasIndex(x => new { x.WarehouseId, x.Code }).IsUnique();
    }
}

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => new { x.OrganizationId, x.Sku }).IsUnique();
    }
}

public sealed class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("product_variants");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Sku).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Barcode).HasMaxLength(64);
        builder.Property(x => x.ReorderPoint)
            .HasConversion(v => v.Value, v => new Quantity(v))
            .HasPrecision(18, 4);
        builder.Property(x => x.ReorderQuantity)
            .HasConversion(v => v.Value, v => new Quantity(v))
            .HasPrecision(18, 4);
        builder.HasIndex(x => new { x.OrganizationId, x.Sku }).IsUnique();
    }
}

public sealed class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.ToTable("stock_levels");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.OnHand)
            .HasConversion(v => v.Value, v => new Quantity(v))
            .HasPrecision(18, 4);
        builder.Property(x => x.Allocated)
            .HasConversion(v => v.Value, v => new Quantity(v))
            .HasPrecision(18, 4);
        builder.Property(x => x.RowVersion).IsRowVersion();
        builder.HasIndex(x => new { x.ProductVariantId, x.LocationId }).IsUnique();
    }
}

public sealed class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("stock_movements");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Quantity)
            .HasConversion(v => v.Value, v => new Quantity(v))
            .HasPrecision(18, 4);
        builder.Property(x => x.ReferenceType).HasMaxLength(64);
        builder.Property(x => x.SerialNumber).HasMaxLength(128);
        builder.HasIndex(x => new { x.OrganizationId, x.ProductVariantId, x.LocationId });
    }
}

public sealed class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("purchase_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(32).IsRequired();
        builder.Property(x => x.VendorName).HasMaxLength(200).IsRequired();
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.PurchaseOrderId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}

public sealed class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("sales_orders");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Number).HasMaxLength(32).IsRequired();
        builder.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
        builder.OwnsOne(x => x.ShippingAddress, a =>
        {
            a.Property(p => p.Line1).HasColumnName("ship_line1").HasMaxLength(200);
            a.Property(p => p.Line2).HasColumnName("ship_line2").HasMaxLength(200);
            a.Property(p => p.City).HasColumnName("ship_city").HasMaxLength(100);
            a.Property(p => p.State).HasColumnName("ship_state").HasMaxLength(100);
            a.Property(p => p.PostalCode).HasColumnName("ship_postal_code").HasMaxLength(20);
            a.Property(p => p.Country).HasColumnName("ship_country").HasMaxLength(100);
        });
        builder.HasMany(x => x.Lines).WithOne().HasForeignKey(x => x.SalesOrderId);
        builder.Navigation(x => x.Lines).HasField("_lines");
    }
}
