using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class Location : OrganizationScopedEntity
{
    public Guid WarehouseId { get; private set; }

    public Guid? ParentLocationId { get; private set; }

    public LocationType Type { get; private set; }

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Barcode { get; private set; }

    public decimal? MaxUnits { get; private set; }

    public decimal? MaxWeightKg { get; private set; }

    public bool IsActive { get; private set; } = true;

    private Location()
    {
    }

    public Location(
        Guid organizationId,
        Guid warehouseId,
        LocationType type,
        string code,
        string name,
        Guid? parentLocationId = null)
        : base(organizationId)
    {
        WarehouseId = warehouseId;
        Type = type;
        SetCode(code);
        SetName(name);
        ParentLocationId = parentLocationId;
    }

    public void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Location code is required.");
        }

        Code = code.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Location name is required.");
        }

        Name = name.Trim();
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

    public void SetCapacity(decimal? maxUnits, decimal? maxWeightKg)
    {
        if (maxUnits is < 0m)
        {
            throw new DomainException("Max units cannot be negative.");
        }

        if (maxWeightKg is < 0m)
        {
            throw new DomainException("Max weight cannot be negative.");
        }

        MaxUnits = maxUnits;
        MaxWeightKg = maxWeightKg;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
