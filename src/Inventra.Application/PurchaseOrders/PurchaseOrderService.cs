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
