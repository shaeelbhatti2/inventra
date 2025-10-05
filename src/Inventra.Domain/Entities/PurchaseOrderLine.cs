using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class PurchaseOrderLine : Entity
{
    public Guid PurchaseOrderId { get; private set; }

    public Guid ProductVariantId { get; private set; }

    public Quantity OrderedQuantity { get; private set; }

    public Quantity ReceivedQuantity { get; private set; } = Quantity.Zero;

    public Money UnitCost { get; private set; }

    private PurchaseOrderLine()
    {
        OrderedQuantity = Quantity.Zero;
        UnitCost = new Money(0m);
    }

    public PurchaseOrderLine(Guid purchaseOrderId, Guid productVariantId, decimal orderedQty, decimal unitCost)
    {
        if (orderedQty <= 0m)
        {
            throw new DomainException("Ordered quantity must be positive.");
        }

        PurchaseOrderId = purchaseOrderId;
        ProductVariantId = productVariantId;
        OrderedQuantity = new Quantity(orderedQty);
        UnitCost = new Money(unitCost);
    }

    public Quantity RemainingQuantity => OrderedQuantity - ReceivedQuantity;

    public bool IsFullyReceived => ReceivedQuantity >= OrderedQuantity;

    public void RecordReceipt(decimal quantity)
    {
        if (quantity <= 0m)
        {
            throw new DomainException("Receipt quantity must be positive.");
        }

        ReceivedQuantity = ReceivedQuantity + new Quantity(quantity);
        Touch();
    }
}
