using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.BatchLots;

public sealed class CreateBatchLotRequest
{
    public Guid OrganizationId { get; init; }
    public Guid ProductVariantId { get; init; }
    public string LotNumber { get; init; } = string.Empty;
    public DateOnly ReceivedDate { get; init; }
    public DateOnly? ExpiryDate { get; init; }
}

public sealed class BatchLotService
{
    private readonly IBatchLotRepository _repository;

    public BatchLotService(IBatchLotRepository repository)
    {
        _repository = repository;
    }

    public async Task<BatchLot> CreateAsync(CreateBatchLotRequest request, CancellationToken ct)
    {
        var existing = await _repository.GetByLotNumberAsync(
            request.OrganizationId,
            request.ProductVariantId,
            request.LotNumber,
            ct);

        if (existing is not null)
        {
            throw new DomainException("Lot number already exists for variant.");
        }

        var lot = new BatchLot(
            request.OrganizationId,
            request.ProductVariantId,
            request.LotNumber,
            request.ReceivedDate,
            request.ExpiryDate);

        await _repository.AddAsync(lot, ct);
        await _repository.SaveChangesAsync(ct);
        return lot;
    }

    public async Task<IReadOnlyList<BatchLot>> SuggestFefoAsync(Guid organizationId, Guid variantId, CancellationToken ct)
    {
        var lots = await _repository.ListByVariantAsync(organizationId, variantId, ct);
        return lots
            .OrderBy(x => x.ExpiryDate ?? DateOnly.MaxValue)
            .ThenBy(x => x.ReceivedDate)
            .ToList();
    }
}
