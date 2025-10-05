using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class StockMovement : OrganizationScopedEntity
{
    public Guid ProductVariantId { get; private set; }

    public Guid LocationId { get; private set; }

    public MovementType Type { get; private set; }

    public Quantity Quantity { get; private set; }

    public Quantity SignedQuantity => Type switch
    {
        MovementType.Receipt or MovementType.TransferIn or MovementType.Adjustment => Quantity,
        MovementType.Shipment or MovementType.TransferOut => new Quantity(-Quantity.Value),
        MovementType.CycleCount => Quantity,
        _ => throw new DomainException("Unknown movement type.")
    };

    public Guid? ReferenceId { get; private set; }

    public string? ReferenceType { get; private set; }

    public Guid? BatchLotId { get; private set; }

    public string? SerialNumber { get; private set; }

    public Money? UnitCost { get; private set; }

    public Guid? PerformedByUserId { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    private StockMovement()
    {
        Quantity = Quantity.Zero;
    }

    public StockMovement(
        Guid organizationId,
        Guid productVariantId,
        Guid locationId,
        MovementType type,
        Quantity quantity,
        DateTimeOffset occurredAt,
        Guid? referenceId = null,
        string? referenceType = null)
        : base(organizationId)
    {
        if (quantity.IsZero)
        {
            throw new DomainException("Movement quantity must be non-zero.");
        }

        ProductVariantId = productVariantId;
        LocationId = locationId;
        Type = type;
        Quantity = quantity.Abs();
        OccurredAt = occurredAt;
        ReferenceId = referenceId;
        ReferenceType = referenceType;
    }

    public void SetBatchLot(Guid batchLotId)
    {
        BatchLotId = batchLotId;
        Touch();
    }

    public void SetSerialNumber(string serialNumber)
    {
        if (string.IsNullOrWhiteSpace(serialNumber))
        {
            throw new DomainException("Serial number is required.");
        }

        SerialNumber = serialNumber.Trim();
        Touch();
    }

    public void SetUnitCost(Money unitCost)
    {
        UnitCost = unitCost;
        Touch();
    }

    public void SetPerformedBy(Guid userId)
    {
        PerformedByUserId = userId;
        Touch();
    }
}
