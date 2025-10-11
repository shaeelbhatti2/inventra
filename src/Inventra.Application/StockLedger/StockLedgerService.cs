using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.StockLedger;

public sealed class RecordMovementRequest
{
    public Guid OrganizationId { get; init; }
    public Guid ProductVariantId { get; init; }
    public Guid LocationId { get; init; }
    public MovementType Type { get; init; }
    public decimal Quantity { get; init; }
    public Guid? ReferenceId { get; init; }
    public string? ReferenceType { get; init; }
    public Money? UnitCost { get; init; }
    public Guid? BatchLotId { get; init; }
    public string? SerialNumber { get; init; }
    public Guid? PerformedByUserId { get; init; }
}

public sealed class StockLedgerService
{
    private readonly IStockLedgerRepository _repository;
    private readonly IOrganizationRepository _organizations;
    private readonly IAuditRepository _audit;

    public StockLedgerService(
        IStockLedgerRepository repository,
        IOrganizationRepository organizations,
        IAuditRepository audit)
    {
        _repository = repository;
        _organizations = organizations;
        _audit = audit;
    }

    public async Task<StockMovement> RecordMovementAsync(RecordMovementRequest request, CancellationToken ct)
    {
        var org = await _organizations.GetByIdAsync(request.OrganizationId, ct)
            ?? throw new DomainException("Organization not found.");

        var qty = new Quantity(request.Quantity);
        if (qty.IsZero)
        {
            throw new DomainException("Quantity must be non-zero.");
        }

        var level = await _repository.GetLevelAsync(
            request.OrganizationId,
            request.ProductVariantId,
            request.LocationId,
            ct);

        var delta = ComputeDelta(request.Type, qty);
        var current = level?.OnHand ?? Quantity.Zero;
        var projected = current + delta;

        if (projected.IsNegative && !org.AllowBackorder)
        {
            throw new DomainException("Insufficient stock at location.");
        }

        var movement = new StockMovement(
            request.OrganizationId,
            request.ProductVariantId,
            request.LocationId,
            request.Type,
            qty,
            DateTimeOffset.UtcNow,
            request.ReferenceId,
            request.ReferenceType);

        if (request.UnitCost.HasValue)
        {
            movement.SetUnitCost(request.UnitCost.Value);
        }

        if (request.BatchLotId.HasValue)
        {
            movement.SetBatchLot(request.BatchLotId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.SerialNumber))
        {
            movement.SetSerialNumber(request.SerialNumber);
        }

        if (request.PerformedByUserId.HasValue)
        {
            movement.SetPerformedBy(request.PerformedByUserId.Value);
        }

        await _repository.AddMovementAsync(movement, ct);
        await _audit.AddAsync(new AuditLogEntry(
            request.OrganizationId,
            nameof(StockMovement),
            movement.Id,
            "recorded",
            $"{request.Type} {request.Quantity}",
            request.PerformedByUserId), ct);
        await _repository.SaveChangesAsync(ct);
        return movement;
    }

    public async Task<Quantity> GetOnHandAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct)
    {
        var level = await _repository.GetLevelAsync(organizationId, variantId, locationId, ct);
        return level?.OnHand ?? Quantity.Zero;
    }

    private static Quantity ComputeDelta(MovementType type, Quantity qty) => type switch
    {
        MovementType.Receipt or MovementType.TransferIn or MovementType.Adjustment => qty,
        MovementType.Shipment or MovementType.TransferOut => new Quantity(-qty.Value),
        MovementType.CycleCount => qty,
        _ => throw new DomainException("Unknown movement type.")
    };
}
