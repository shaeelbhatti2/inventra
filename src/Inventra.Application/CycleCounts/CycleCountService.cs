using Inventra.Application.Abstractions;
using Inventra.Application.StockLedger;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.CycleCounts;

public sealed class CreateCycleCountRequest
{
    public Guid OrganizationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public bool BlindCount { get; init; }
    public IReadOnlyList<CycleCountLineSeed> Lines { get; init; } = Array.Empty<CycleCountLineSeed>();
}

public sealed class CycleCountLineSeed
{
    public Guid ProductVariantId { get; init; }
    public Guid LocationId { get; init; }
    public decimal ExpectedQuantity { get; init; }
}

public sealed class SubmitCountLineRequest
{
    public Guid OrganizationId { get; set; }
    public Guid CycleCountId { get; set; }
    public Guid LineId { get; set; }
    public decimal CountedQuantity { get; set; }
}

public sealed class CycleCountService
{
    private readonly ICycleCountRepository _repository;
    private readonly StockLedgerService _ledger;

    public CycleCountService(ICycleCountRepository repository, StockLedgerService ledger)
    {
        _repository = repository;
        _ledger = ledger;
    }

    public async Task<CycleCount> CreateAsync(CreateCycleCountRequest request, CancellationToken ct)
    {
        var count = new CycleCount(request.OrganizationId, request.Name, request.BlindCount);
        foreach (var line in request.Lines)
        {
            count.AddLine(line.ProductVariantId, line.LocationId, line.ExpectedQuantity);
        }

        count.Start();
        await _repository.AddAsync(count, ct);
        await _repository.SaveChangesAsync(ct);
        return count;
    }

    public async Task SubmitLineAsync(SubmitCountLineRequest request, CancellationToken ct)
    {
        var count = await _repository.GetByIdAsync(request.OrganizationId, request.CycleCountId, ct)
            ?? throw new DomainException("Cycle count not found.");

        var line = count.Lines.FirstOrDefault(x => x.Id == request.LineId)
            ?? throw new DomainException("Line not found.");

        line.SubmitCount(request.CountedQuantity);
        await _repository.SaveChangesAsync(ct);
    }

    public async Task ApproveLineAsync(Guid organizationId, Guid countId, Guid lineId, Guid? userId, CancellationToken ct)
    {
        var count = await _repository.GetByIdAsync(organizationId, countId, ct)
            ?? throw new DomainException("Cycle count not found.");

        var line = count.Lines.FirstOrDefault(x => x.Id == lineId)
            ?? throw new DomainException("Line not found.");

        line.Approve();

        var variance = line.Variance;
        if (!variance.IsZero)
        {
            await _ledger.RecordMovementAsync(new RecordMovementRequest
            {
                OrganizationId = organizationId,
                ProductVariantId = line.ProductVariantId,
                LocationId = line.LocationId,
                Type = MovementType.Adjustment,
                Quantity = variance.Value,
                ReferenceId = count.Id,
                ReferenceType = nameof(CycleCount),
                PerformedByUserId = userId
            }, ct);
        }

        if (count.Lines.All(x => x.Approved))
        {
            count.Complete();
        }
        else
        {
            count.SubmitForApproval();
        }

        await _repository.SaveChangesAsync(ct);
    }
}
