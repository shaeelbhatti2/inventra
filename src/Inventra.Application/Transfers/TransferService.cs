using Inventra.Application.Abstractions;
using Inventra.Application.StockLedger;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.Transfers;

public sealed class CreateTransferRequest
{
    public Guid OrganizationId { get; init; }
    public string Number { get; init; } = string.Empty;
    public Guid FromLocationId { get; init; }
    public Guid ToLocationId { get; init; }
    public Guid ProductVariantId { get; init; }
    public decimal Quantity { get; init; }
    public Guid? PerformedByUserId { get; init; }
}

public sealed class TransferService
{
    private readonly ITransferOrderRepository _repository;
    private readonly StockLedgerService _ledger;

    public TransferService(ITransferOrderRepository repository, StockLedgerService ledger)
    {
        _repository = repository;
        _ledger = ledger;
    }

    public async Task<TransferOrder> CreateAndShipAsync(CreateTransferRequest request, CancellationToken ct)
    {
        var transfer = new TransferOrder(
            request.OrganizationId,
            request.Number,
            request.FromLocationId,
            request.ToLocationId);

        transfer.MarkPicked();
        transfer.MarkInTransit();

        await _ledger.RecordMovementAsync(new RecordMovementRequest
        {
            OrganizationId = request.OrganizationId,
            ProductVariantId = request.ProductVariantId,
            LocationId = request.FromLocationId,
            Type = MovementType.TransferOut,
            Quantity = request.Quantity,
            ReferenceId = transfer.Id,
            ReferenceType = nameof(TransferOrder),
            PerformedByUserId = request.PerformedByUserId
        }, ct);

        await _repository.AddAsync(transfer, ct);
        await _repository.SaveChangesAsync(ct);
        return transfer;
    }

    public async Task ReceiveAsync(Guid organizationId, Guid transferId, Guid variantId, decimal quantity, Guid? userId, CancellationToken ct)
    {
        var transfer = await _repository.GetByIdAsync(organizationId, transferId, ct)
            ?? throw new DomainException("Transfer not found.");

        if (transfer.Status != TransferOrderStatus.InTransit)
        {
            throw new DomainException("Transfer is not in transit.");
        }

        await _ledger.RecordMovementAsync(new RecordMovementRequest
        {
            OrganizationId = organizationId,
            ProductVariantId = variantId,
            LocationId = transfer.ToLocationId,
            Type = MovementType.TransferIn,
            Quantity = quantity,
            ReferenceId = transfer.Id,
            ReferenceType = nameof(TransferOrder),
            PerformedByUserId = userId
        }, ct);

        transfer.MarkReceived();
        await _repository.SaveChangesAsync(ct);
    }
}
