using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.PurchaseOrders;

public sealed class CreatePurchaseOrderRequest
{
    public Guid OrganizationId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string VendorName { get; init; } = string.Empty;
    public Guid ReceivingLocationId { get; init; }
    public DateOnly? ExpectedDeliveryDate { get; init; }
    public IReadOnlyList<PurchaseOrderLineRequest> Lines { get; init; } = Array.Empty<PurchaseOrderLineRequest>();
}

public sealed class PurchaseOrderLineRequest
{
    public Guid ProductVariantId { get; init; }
    public decimal Quantity { get; init; }
    public decimal UnitCost { get; init; }
}

public sealed class PurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repository;

    public PurchaseOrderService(IPurchaseOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<PurchaseOrder> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken ct)
    {
        var order = new PurchaseOrder(request.OrganizationId, request.Number, request.VendorName);
        order.SetReceivingLocation(request.ReceivingLocationId);
        order.SetExpectedDeliveryDate(request.ExpectedDeliveryDate);

        foreach (var line in request.Lines)
        {
            order.AddLine(line.ProductVariantId, line.Quantity, line.UnitCost);
        }

        await _repository.AddAsync(order, ct);
        await _repository.SaveChangesAsync(ct);
        return order;
    }

    public async Task SubmitAsync(Guid organizationId, Guid orderId, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(organizationId, orderId, ct)
            ?? throw new DomainException("Purchase order not found.");

        order.Submit();
        await _repository.SaveChangesAsync(ct);
    }

    public async Task MarkOrderedAsync(Guid organizationId, Guid orderId, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(organizationId, orderId, ct)
            ?? throw new DomainException("Purchase order not found.");

        order.MarkOrdered();
        await _repository.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<PurchaseOrder>> ListAsync(Guid organizationId, CancellationToken ct) =>
        _repository.ListAsync(organizationId, ct);
}

public sealed class ReceivePurchaseOrderRequest
{
    public Guid OrganizationId { get; set; }
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductVariantId { get; set; }
    public decimal Quantity { get; set; }
    public Guid? PerformedByUserId { get; set; }
}

public sealed class PurchaseOrderReceivingService
{
    private readonly IPurchaseOrderRepository _orders;
    private readonly StockLedger.StockLedgerService _ledger;
    private readonly IOrganizationRepository _organizations;

    public PurchaseOrderReceivingService(
        IPurchaseOrderRepository orders,
        StockLedger.StockLedgerService ledger,
        IOrganizationRepository organizations)
    {
        _orders = orders;
        _ledger = ledger;
        _organizations = organizations;
    }

    public async Task ReceiveLineAsync(ReceivePurchaseOrderRequest request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrganizationId, request.PurchaseOrderId, ct)
            ?? throw new DomainException("Purchase order not found.");

        if (order.Status is not (PurchaseOrderStatus.Ordered or PurchaseOrderStatus.PartiallyReceived))
        {
            throw new DomainException("Purchase order is not receivable.");
        }

        var line = order.Lines.FirstOrDefault(x => x.ProductVariantId == request.ProductVariantId)
            ?? throw new DomainException("Line not found on purchase order.");

        var org = await _organizations.GetByIdAsync(request.OrganizationId, ct)
            ?? throw new DomainException("Organization not found.");

        var maxAllowed = line.OrderedQuantity.Value * (1m + org.OverReceiptTolerancePercent / 100m);
        if (line.ReceivedQuantity.Value + request.Quantity > maxAllowed)
        {
            throw new DomainException("Receipt exceeds allowed tolerance.");
        }

        line.RecordReceipt(request.Quantity);

        await _ledger.RecordMovementAsync(new StockLedger.RecordMovementRequest
        {
            OrganizationId = request.OrganizationId,
            ProductVariantId = request.ProductVariantId,
            LocationId = order.ReceivingLocationId!.Value,
            Type = MovementType.Receipt,
            Quantity = request.Quantity,
            ReferenceId = order.Id,
            ReferenceType = nameof(PurchaseOrder),
            UnitCost = line.UnitCost,
            PerformedByUserId = request.PerformedByUserId
        }, ct);

        if (line.IsFullyReceived && order.Lines.All(x => x.IsFullyReceived))
        {
            order.MarkReceived();
        }
        else
        {
            order.MarkPartiallyReceived();
        }

        await _orders.SaveChangesAsync(ct);
    }
}
