using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class ProductVariant : OrganizationScopedEntity
{
    public Guid ProductId { get; private set; }

    public string Sku { get; private set; } = string.Empty;

    public string? Size { get; private set; }

    public string? Color { get; private set; }

    public string? Barcode { get; private set; }

    public UnitOfMeasure UnitOfMeasure { get; private set; } = UnitOfMeasure.Each;

    public decimal? WeightKg { get; private set; }

    public decimal? LengthCm { get; private set; }

    public decimal? WidthCm { get; private set; }

    public decimal? HeightCm { get; private set; }

    public Quantity ReorderPoint { get; private set; } = Quantity.Zero;

    public Quantity ReorderQuantity { get; private set; } = Quantity.Zero;

    public bool TrackLots { get; private set; }

    public bool TrackSerials { get; private set; }

    private ProductVariant()
    {
    }

    public ProductVariant(Guid organizationId, Guid productId, string sku)
        : base(organizationId)
    {
        ProductId = productId;
        SetSku(sku);
    }

    public void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("Variant SKU is required.");
        }

        Sku = sku.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetAttributes(string? size, string? color)
    {
        Size = size?.Trim();
        Color = color?.Trim();
        Touch();
    }

    public void AssignBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new DomainException("Barcode is required.");
        }

        Barcode = barcode.Trim();
        Touch();
    }

    public void SetUnitOfMeasure(UnitOfMeasure unit)
    {
        UnitOfMeasure = unit;
        Touch();
    }

    public void SetDimensions(decimal? weightKg, decimal? lengthCm, decimal? widthCm, decimal? heightCm)
    {
        WeightKg = weightKg;
        LengthCm = lengthCm;
        WidthCm = widthCm;
        HeightCm = heightCm;
        Touch();
    }

    public void SetReorderLevels(Quantity reorderPoint, Quantity reorderQuantity)
    {
        reorderPoint.EnsureNonNegative();
        reorderQuantity.EnsureNonNegative();
        ReorderPoint = reorderPoint;
        ReorderQuantity = reorderQuantity;
        Touch();
    }

    public void SetTracking(bool trackLots, bool trackSerials)
    {
        TrackLots = trackLots;
        TrackSerials = trackSerials;
        Touch();
    }
}
