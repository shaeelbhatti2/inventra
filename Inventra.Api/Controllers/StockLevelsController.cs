using Inventra.Application.StockLedger;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/stock-levels")]
public sealed class StockLevelsController : ControllerBase
{
    [HttpGet("{variantId:guid}")]
    public async Task<IActionResult> Get(
        Guid variantId,
        [FromQuery] Guid organizationId,
        [FromQuery] Guid locationId,
        [FromServices] StockLedgerService ledger,
        CancellationToken ct)
    {
        var qty = await ledger.GetOnHandAsync(organizationId, variantId, locationId, ct);
        Response.Headers.ETag = $"\"{qty.Value}\"";
        return Ok(new { onHand = qty.Value, variantId, locationId });
    }
}
