using Inventra.Domain.Common;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class StockLevel : OrganizationScopedEntity
{
    public Guid ProductVariantId { get; private set; }

    public Guid LocationId { get; private set; }

    public Quantity OnHand { get; private set; } = Quantity.Zero;

    public Quantity Allocated { get; private set; } = Quantity.Zero;

    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();

    private StockLevel()
    {
    }

    public StockLevel(Guid organizationId, Guid productVariantId, Guid locationId)
        : base(organizationId)
    {
        ProductVariantId = productVariantId;
        LocationId = locationId;
    }

    public Quantity Available => OnHand - Allocated;

    internal void ApplyMovement(Quantity delta)
    {
        OnHand = OnHand + delta;
        Touch();
    }

    internal void Allocate(Quantity quantity)
    {
        Allocated = Allocated + quantity;
        Touch();
    }

    internal void ReleaseAllocation(Quantity quantity)
    {
        Allocated = Allocated - quantity;
        Touch();
    }
}
