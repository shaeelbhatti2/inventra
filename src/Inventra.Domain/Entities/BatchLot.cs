using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class BatchLot : OrganizationScopedEntity
{
    public Guid ProductVariantId { get; private set; }

    public string LotNumber { get; private set; } = string.Empty;

    public DateOnly? ExpiryDate { get; private set; }

    public DateOnly ReceivedDate { get; private set; }

    private BatchLot()
    {
    }

    public BatchLot(Guid organizationId, Guid productVariantId, string lotNumber, DateOnly receivedDate, DateOnly? expiryDate)
        : base(organizationId)
    {
        ProductVariantId = productVariantId;
        SetLotNumber(lotNumber);
        ReceivedDate = receivedDate;
        ExpiryDate = expiryDate;
    }

    public void SetLotNumber(string lotNumber)
    {
        if (string.IsNullOrWhiteSpace(lotNumber))
        {
            throw new DomainException("Lot number is required.");
        }

        LotNumber = lotNumber.Trim().ToUpperInvariant();
        Touch();
    }
}
