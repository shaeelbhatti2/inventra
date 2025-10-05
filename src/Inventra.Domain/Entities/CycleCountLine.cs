using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class CycleCountLine : Entity
{
    public Guid CycleCountId { get; private set; }

    public Guid ProductVariantId { get; private set; }

    public Guid LocationId { get; private set; }

    public Quantity ExpectedQuantity { get; private set; }

    public Quantity? CountedQuantity { get; private set; }

    public bool Approved { get; private set; }

    private CycleCountLine()
    {
        ExpectedQuantity = Quantity.Zero;
    }

    public CycleCountLine(Guid cycleCountId, Guid productVariantId, Guid locationId, decimal expectedQty)
    {
        CycleCountId = cycleCountId;
        ProductVariantId = productVariantId;
        LocationId = locationId;
        ExpectedQuantity = new Quantity(expectedQty);
    }

    public Quantity Variance =>
        CountedQuantity.HasValue
            ? CountedQuantity.Value - ExpectedQuantity
            : Quantity.Zero;

    public void SubmitCount(decimal countedQty)
    {
        CountedQuantity = new Quantity(countedQty);
        Touch();
    }

    public void Approve()
    {
        if (!CountedQuantity.HasValue)
        {
            throw new DomainException("Line must be counted before approval.");
        }

        Approved = true;
        Touch();
    }
}
