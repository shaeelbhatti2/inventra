using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class TransferOrderRepository : ITransferOrderRepository
{
    private readonly InventraDbContext _db;

    public TransferOrderRepository(InventraDbContext db) => _db = db;

    public Task<TransferOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.TransferOrders.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task AddAsync(TransferOrder order, CancellationToken ct) =>
        await _db.TransferOrders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class CycleCountRepository : ICycleCountRepository
{
    private readonly InventraDbContext _db;

    public CycleCountRepository(InventraDbContext db) => _db = db;

    public Task<CycleCount?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.CycleCounts.Include(x => x.Lines).FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task AddAsync(CycleCount count, CancellationToken ct) =>
        await _db.CycleCounts.AddAsync(count, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class BatchLotRepository : IBatchLotRepository
{
    private readonly InventraDbContext _db;

    public BatchLotRepository(InventraDbContext db) => _db = db;

    public Task<BatchLot?> GetByLotNumberAsync(Guid organizationId, Guid variantId, string lotNumber, CancellationToken ct) =>
        _db.BatchLots.FirstOrDefaultAsync(x =>
            x.OrganizationId == organizationId &&
            x.ProductVariantId == variantId &&
            x.LotNumber == lotNumber.ToUpperInvariant(), ct);

    public async Task<IReadOnlyList<BatchLot>> ListByVariantAsync(Guid organizationId, Guid variantId, CancellationToken ct) =>
        await _db.BatchLots.Where(x => x.OrganizationId == organizationId && x.ProductVariantId == variantId).ToListAsync(ct);

    public async Task AddAsync(BatchLot lot, CancellationToken ct) =>
        await _db.BatchLots.AddAsync(lot, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class SerialNumberRepository : ISerialNumberRepository
{
    private readonly InventraDbContext _db;

    public SerialNumberRepository(InventraDbContext db) => _db = db;

    public Task<SerialNumber?> GetByNumberAsync(Guid organizationId, string number, CancellationToken ct) =>
        _db.SerialNumbers.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Number == number, ct);

    public async Task AddAsync(SerialNumber serial, CancellationToken ct) =>
        await _db.SerialNumbers.AddAsync(serial, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
