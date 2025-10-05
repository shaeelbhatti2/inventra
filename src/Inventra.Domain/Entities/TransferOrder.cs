using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class TransferOrder : OrganizationScopedEntity
{
    public string Number { get; private set; } = string.Empty;

    public Guid FromLocationId { get; private set; }

    public Guid ToLocationId { get; private set; }

    public TransferOrderStatus Status { get; private set; } = TransferOrderStatus.Draft;

    public DateTimeOffset? ShippedAt { get; private set; }

    public DateTimeOffset? ReceivedAt { get; private set; }

    private TransferOrder()
    {
    }

    public TransferOrder(Guid organizationId, string number, Guid fromLocationId, Guid toLocationId)
        : base(organizationId)
    {
        if (fromLocationId == toLocationId)
        {
            throw new DomainException("Source and destination must differ.");
        }

        SetNumber(number);
        FromLocationId = fromLocationId;
        ToLocationId = toLocationId;
    }

    public void SetNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("Transfer number is required.");
        }

        Number = number.Trim();
        Touch();
    }

    public void MarkPicked()
    {
        EnsureStatus(TransferOrderStatus.Draft);
        Status = TransferOrderStatus.Picked;
        Touch();
    }

    public void MarkInTransit()
    {
        EnsureStatus(TransferOrderStatus.Picked);
        Status = TransferOrderStatus.InTransit;
        ShippedAt = DateTimeOffset.UtcNow;
        Touch();
    }

    public void MarkReceived()
    {
        if (Status != TransferOrderStatus.InTransit)
        {
            throw new DomainException("Transfer must be in transit before receiving.");
        }

        Status = TransferOrderStatus.Received;
        ReceivedAt = DateTimeOffset.UtcNow;
        Touch();
    }

    public void Cancel()
    {
        if (Status is TransferOrderStatus.InTransit or TransferOrderStatus.Received)
        {
            throw new DomainException("Cannot cancel an in-transit or received transfer.");
        }

        Status = TransferOrderStatus.Cancelled;
        Touch();
    }

    private void EnsureStatus(TransferOrderStatus expected)
    {
        if (Status != expected)
        {
            throw new DomainException($"Expected transfer status {expected} but was {Status}.");
        }
    }
}
