using Inventra.Application.PurchaseOrders;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/purchase-orders")]
public sealed class PurchaseOrderReceivingController : ControllerBase
{
    private readonly PurchaseOrderReceivingService _receiving;

    public PurchaseOrderReceivingController(PurchaseOrderReceivingService receiving) => _receiving = receiving;

    [HttpPost("{id:guid}/receive")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePurchaseOrderRequest request, CancellationToken ct)
    {
        request.PurchaseOrderId = id;
        await _receiving.ReceiveLineAsync(request, ct);
        return NoContent();
    }
}
