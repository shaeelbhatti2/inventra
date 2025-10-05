using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class PurchaseOrder : OrganizationScopedEntity
{
    public string Number { get; private set; } = string.Empty;

    public string VendorName { get; private set; } = string.Empty;

    public PurchaseOrderStatus Status { get; private set; } = PurchaseOrderStatus.Draft;

    public Guid? ReceivingLocationId { get; private set; }

    public DateOnly? ExpectedDeliveryDate { get; private set; }

    private readonly List<PurchaseOrderLine> _lines = new();

    public IReadOnlyCollection<PurchaseOrderLine> Lines => _lines.AsReadOnly();

    private PurchaseOrder()
    {
    }

    public PurchaseOrder(Guid organizationId, string number, string vendorName)
        : base(organizationId)
    {
        SetNumber(number);
        SetVendorName(vendorName);
    }

    public void SetNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("PO number is required.");
        }

        Number = number.Trim();
        Touch();
    }

    public void SetVendorName(string vendorName)
    {
        if (string.IsNullOrWhiteSpace(vendorName))
        {
            throw new DomainException("Vendor name is required.");
        }

        VendorName = vendorName.Trim();
        Touch();
    }

    public void SetReceivingLocation(Guid locationId)
    {
        ReceivingLocationId = locationId;
        Touch();
    }

    public void SetExpectedDeliveryDate(DateOnly? date)
    {
        ExpectedDeliveryDate = date;
        Touch();
    }

    public PurchaseOrderLine AddLine(Guid productVariantId, decimal orderedQty, decimal unitCost)
    {
        EnsureEditable();
        var line = new PurchaseOrderLine(Id, productVariantId, orderedQty, unitCost);
        _lines.Add(line);
        Touch();
        return line;
    }

    public void Submit()
    {
        EnsureStatus(PurchaseOrderStatus.Draft);
        if (_lines.Count == 0)
        {
            throw new DomainException("Purchase order must have at least one line.");
        }

        Status = PurchaseOrderStatus.Submitted;
        Touch();
    }

    public void MarkOrdered()
    {
        EnsureStatus(PurchaseOrderStatus.Submitted);
        Status = PurchaseOrderStatus.Ordered;
        Touch();
    }

    public void Cancel()
    {
        if (Status is PurchaseOrderStatus.PartiallyReceived or PurchaseOrderStatus.Received)
        {
            throw new DomainException("Cannot cancel a PO after receipt started.");
        }

        Status = PurchaseOrderStatus.Cancelled;
        Touch();
    }

    public void MarkPartiallyReceived()
    {
        Status = PurchaseOrderStatus.PartiallyReceived;
        Touch();
    }

    public void MarkReceived()
    {
        Status = PurchaseOrderStatus.Received;
        Touch();
    }

    public void Close()
    {
        Status = PurchaseOrderStatus.Closed;
        Touch();
    }

    private void EnsureEditable()
    {
        if (Status != PurchaseOrderStatus.Draft)
        {
            throw new DomainException("Purchase order is not editable.");
        }
    }

    private void EnsureStatus(PurchaseOrderStatus expected)
    {
        if (Status != expected)
        {
            throw new DomainException($"Expected PO status {expected} but was {Status}.");
        }
    }
}
