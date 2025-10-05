using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class SalesOrderLine : Entity
{
    public Guid SalesOrderId { get; private set; }

    public Guid ProductVariantId { get; private set; }

    public Quantity OrderedQuantity { get; private set; }

    public Quantity AllocatedQuantity { get; private set; } = Quantity.Zero;

    public Quantity PickedQuantity { get; private set; } = Quantity.Zero;

    public Quantity ShippedQuantity { get; private set; } = Quantity.Zero;

    private SalesOrderLine()
    {
        OrderedQuantity = Quantity.Zero;
    }

    public SalesOrderLine(Guid salesOrderId, Guid productVariantId, decimal quantity)
    {
        if (quantity <= 0m)
        {
            throw new DomainException("Line quantity must be positive.");
        }

        SalesOrderId = salesOrderId;
        ProductVariantId = productVariantId;
        OrderedQuantity = new Quantity(quantity);
    }

    public Quantity RemainingToAllocate => OrderedQuantity - AllocatedQuantity;

    public void Allocate(decimal quantity)
    {
        var qty = new Quantity(quantity);
        if (qty > RemainingToAllocate)
        {
            throw new DomainException("Cannot allocate more than ordered quantity.");
        }

        AllocatedQuantity = AllocatedQuantity + qty;
        Touch();
    }

    public void RecordPick(decimal quantity)
    {
        var qty = new Quantity(quantity);
        PickedQuantity = PickedQuantity + qty;
        Touch();
    }

    public void RecordShipment(decimal quantity)
    {
        var qty = new Quantity(quantity);
        ShippedQuantity = ShippedQuantity + qty;
        Touch();
    }
}
