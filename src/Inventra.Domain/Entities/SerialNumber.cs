using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class SerialNumber : OrganizationScopedEntity
{
    public Guid ProductVariantId { get; private set; }

    public string Number { get; private set; } = string.Empty;

    public Guid? CurrentLocationId { get; private set; }

    public bool IsShipped { get; private set; }

    public DateTimeOffset? ShippedAt { get; private set; }

    private SerialNumber()
    {
    }

    public SerialNumber(Guid organizationId, Guid productVariantId, string number, Guid locationId)
        : base(organizationId)
    {
        ProductVariantId = productVariantId;
        SetNumber(number);
        CurrentLocationId = locationId;
    }

    public void SetNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Serial number is required.");
        }

        Number = number.Trim();
        Touch();
    }

    public void MoveToLocation(Guid locationId)
    {
        CurrentLocationId = locationId;
        Touch();
    }

    public void MarkShipped()
    {
        IsShipped = true;
        ShippedAt = DateTimeOffset.UtcNow;
        CurrentLocationId = null;
        Touch();
    }
}
