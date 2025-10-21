using Inventra.Application.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public sealed class PurchaseOrdersController : ControllerBase
{
    private readonly PurchaseOrderService _orders;

    public PurchaseOrdersController(PurchaseOrderService orders) => _orders = orders;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _orders.ListAsync(organizationId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderRequest request, CancellationToken ct) =>
        Ok(await _orders.CreateAsync(request, ct));
}
