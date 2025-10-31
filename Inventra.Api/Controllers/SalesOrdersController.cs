using Inventra.Application.SalesOrders;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/sales-orders")]
public sealed class SalesOrdersController : ControllerBase
{
    private readonly SalesOrderService _orders;
    private readonly AllocationService _allocation;

    public SalesOrdersController(SalesOrderService orders, AllocationService allocation)
    {
        _orders = orders;
        _allocation = allocation;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _orders.ListAsync(organizationId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderRequest request, CancellationToken ct) =>
        Ok(await _orders.CreateAsync(request, ct));

    [HttpPost("{id:guid}/allocate")]
    public async Task<IActionResult> Allocate(Guid id, [FromBody] AllocateStockRequest request, CancellationToken ct)
    {
        request.SalesOrderId = id;
        await _allocation.AllocateAsync(request, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/pick-list")]
    public async Task<IActionResult> PickList(Guid id, [FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _allocation.BuildPickListAsync(organizationId, id, ct));
}
