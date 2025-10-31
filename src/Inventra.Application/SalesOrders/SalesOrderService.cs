using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.SalesOrders;

public sealed class CreateSalesOrderRequest
{
    public Guid OrganizationId { get; init; }
    public string Number { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public Address ShippingAddress { get; init; }
    public IReadOnlyList<SalesOrderLineRequest> Lines { get; init; } = Array.Empty<SalesOrderLineRequest>();
}

public sealed class SalesOrderLineRequest
{
    public Guid ProductVariantId { get; init; }
    public decimal Quantity { get; init; }
}

public sealed class SalesOrderService
{
    private readonly ISalesOrderRepository _repository;

    public SalesOrderService(ISalesOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<SalesOrder> CreateAsync(CreateSalesOrderRequest request, CancellationToken ct)
    {
        var order = new SalesOrder(
            request.OrganizationId,
            request.Number,
            request.CustomerName,
            request.ShippingAddress);

        foreach (var line in request.Lines)
        {
            order.AddLine(line.ProductVariantId, line.Quantity);
        }

        await _repository.AddAsync(order, ct);
        await _repository.SaveChangesAsync(ct);
        return order;
    }

    public async Task ConfirmAsync(Guid organizationId, Guid orderId, CancellationToken ct)
    {
        var order = await _repository.GetByIdAsync(organizationId, orderId, ct)
            ?? throw new DomainException("Sales order not found.");

        order.Confirm();
        await _repository.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<SalesOrder>> ListAsync(Guid organizationId, CancellationToken ct) =>
        _repository.ListAsync(organizationId, ct);
}

public sealed class AllocateStockRequest
{
    public Guid OrganizationId { get; set; }
    public Guid SalesOrderId { get; set; }
    public Guid SalesOrderLineId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Quantity { get; set; }
}

public sealed class PickListLine
{
    public Guid SalesOrderLineId { get; init; }
    public Guid ProductVariantId { get; init; }
    public string LocationCode { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
}

public sealed class AllocationService
{
    private readonly ISalesOrderRepository _orders;
    private readonly IStockLedgerRepository _stock;
    private readonly IWarehouseRepository _warehouses;

    public AllocationService(
        ISalesOrderRepository orders,
        IStockLedgerRepository stock,
        IWarehouseRepository warehouses)
    {
        _orders = orders;
        _stock = stock;
        _warehouses = warehouses;
    }

    public async Task AllocateAsync(AllocateStockRequest request, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(request.OrganizationId, request.SalesOrderId, ct)
            ?? throw new DomainException("Sales order not found.");

        if (order.Status != SalesOrderStatus.Confirmed)
        {
            throw new DomainException("Order must be confirmed before allocation.");
        }

        var line = order.Lines.FirstOrDefault(x => x.Id == request.SalesOrderLineId)
            ?? throw new DomainException("Line not found.");

        var level = await _stock.GetLevelAsync(
            request.OrganizationId,
            line.ProductVariantId,
            request.LocationId,
            ct) ?? throw new DomainException("No stock at location.");

        var qty = new Quantity(request.Quantity);
        if (qty > level.Available)
        {
            throw new DomainException("Insufficient available stock.");
        }

        line.Allocate(request.Quantity);
        level.Allocate(qty);
        order.StartPicking();
        await _orders.SaveChangesAsync(ct);
        await _stock.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<PickListLine>> BuildPickListAsync(Guid organizationId, Guid orderId, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(organizationId, orderId, ct)
            ?? throw new DomainException("Sales order not found.");

        var lines = new List<PickListLine>();
        foreach (var line in order.Lines.Where(x => x.AllocatedQuantity.Value > 0))
        {
            var locations = await _warehouses.ListLocationsAsync(organizationId, Guid.Empty, ct);
            var location = locations.FirstOrDefault();
            lines.Add(new PickListLine
            {
                SalesOrderLineId = line.Id,
                ProductVariantId = line.ProductVariantId,
                LocationCode = location?.Code ?? "UNKNOWN",
                Quantity = line.AllocatedQuantity.Value
            });
        }

        return lines.OrderBy(x => x.LocationCode).ToList();
    }
}
