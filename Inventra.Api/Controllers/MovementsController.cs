using Inventra.Application.PurchaseOrders;
using Inventra.Application.StockLedger;
using Inventra.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/movements")]
[Authorize]
public sealed class MovementsController : ControllerBase
{
    private readonly StockLedgerService _ledger;
    private readonly PurchaseOrderReceivingService _receiving;

    public MovementsController(StockLedgerService ledger, PurchaseOrderReceivingService receiving)
    {
        _ledger = ledger;
        _receiving = receiving;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> Receive([FromBody] ReceivePurchaseOrderRequest request, CancellationToken ct)
    {
        await _receiving.ReceiveLineAsync(request, ct);
        return NoContent();
    }

    [HttpPost("pick")]
    public async Task<IActionResult> Pick([FromBody] PickMovementRequest request, CancellationToken ct)
    {
        await _ledger.RecordMovementAsync(new RecordMovementRequest
        {
            OrganizationId = request.OrganizationId,
            ProductVariantId = request.ProductVariantId,
            LocationId = request.LocationId,
            Type = MovementType.Shipment,
            Quantity = request.Quantity,
            ReferenceId = request.ReferenceId,
            ReferenceType = request.ReferenceType,
            PerformedByUserId = request.PerformedByUserId
        }, ct);
        return NoContent();
    }
}

public sealed class PickMovementRequest
{
    public Guid OrganizationId { get; set; }
    public Guid ProductVariantId { get; set; }
    public Guid LocationId { get; set; }
    public decimal Quantity { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? PerformedByUserId { get; set; }
}
