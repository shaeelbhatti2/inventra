using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class SalesOrder : OrganizationScopedEntity
{
    public string Number { get; private set; } = string.Empty;

    public string CustomerName { get; private set; } = string.Empty;

    public Address ShippingAddress { get; private set; }

    public SalesOrderStatus Status { get; private set; } = SalesOrderStatus.Draft;

    private readonly List<SalesOrderLine> _lines = new();

    public IReadOnlyCollection<SalesOrderLine> Lines => _lines.AsReadOnly();

    private SalesOrder()
    {
        ShippingAddress = new Address(string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty);
    }

    public SalesOrder(Guid organizationId, string number, string customerName, Address shippingAddress)
        : base(organizationId)
    {
        SetNumber(number);
        SetCustomerName(customerName);
        ShippingAddress = shippingAddress;
    }

    public void SetNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("SO number is required.");
        }

        Number = number.Trim();
        Touch();
    }

    public void SetCustomerName(string customerName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new DomainException("Customer name is required.");
        }

        CustomerName = customerName.Trim();
        Touch();
    }

    public SalesOrderLine AddLine(Guid productVariantId, decimal quantity)
    {
        EnsureEditable();
        var line = new SalesOrderLine(Id, productVariantId, quantity);
        _lines.Add(line);
        Touch();
        return line;
    }

    public void Confirm()
    {
        EnsureStatus(SalesOrderStatus.Draft);
        if (_lines.Count == 0)
        {
            throw new DomainException("Sales order must have at least one line.");
        }

        Status = SalesOrderStatus.Confirmed;
        Touch();
    }

    public void StartPicking()
    {
        EnsureStatus(SalesOrderStatus.Confirmed);
        Status = SalesOrderStatus.Picking;
        Touch();
    }

    public void MarkPacked()
    {
        EnsureStatus(SalesOrderStatus.Picking);
        Status = SalesOrderStatus.Packed;
        Touch();
    }

    public void MarkShipped()
    {
        if (Status is SalesOrderStatus.Shipped or SalesOrderStatus.Delivered)
        {
            throw new DomainException("Order already shipped.");
        }

        if (Status == SalesOrderStatus.Cancelled)
        {
            throw new DomainException("Cannot ship a cancelled order.");
        }

        Status = SalesOrderStatus.Shipped;
        Touch();
    }

    public void Cancel()
    {
        if (Status is SalesOrderStatus.Shipped or SalesOrderStatus.Delivered)
        {
            throw new DomainException("Cannot cancel after shipment.");
        }

        Status = SalesOrderStatus.Cancelled;
        Touch();
    }

    private void EnsureEditable()
    {
        if (Status != SalesOrderStatus.Draft)
        {
            throw new DomainException("Sales order is not editable.");
        }
    }

    private void EnsureStatus(SalesOrderStatus expected)
    {
        if (Status != expected)
        {
            throw new DomainException($"Expected SO status {expected} but was {Status}.");
        }
    }
}
