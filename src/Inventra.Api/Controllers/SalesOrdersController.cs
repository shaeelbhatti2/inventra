using Inventra.Application.SalesOrders;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/sales-orders")]
public sealed class SalesOrdersController : ControllerBase
{
    private readonly SalesOrderService _orders;

    public SalesOrdersController(SalesOrderService orders) => _orders = orders;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _orders.ListAsync(organizationId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderRequest request, CancellationToken ct) =>
        Ok(await _orders.CreateAsync(request, ct));
}
