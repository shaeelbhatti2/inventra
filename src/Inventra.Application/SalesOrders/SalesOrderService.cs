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
