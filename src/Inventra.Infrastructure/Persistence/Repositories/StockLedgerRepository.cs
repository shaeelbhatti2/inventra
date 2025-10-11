using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class StockLedgerRepository : IStockLedgerRepository
{
    private readonly InventraDbContext _db;

    public StockLedgerRepository(InventraDbContext db)
    {
        _db = db;
    }

    public async Task<StockLevel?> GetLevelAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct)
    {
        return await _db.StockLevels.FirstOrDefaultAsync(
            x => x.OrganizationId == organizationId && x.ProductVariantId == variantId && x.LocationId == locationId,
            ct);
    }

    public async Task AddMovementAsync(StockMovement movement, CancellationToken ct)
    {
        await _db.StockMovements.AddAsync(movement, ct);
        var level = await GetLevelAsync(movement.OrganizationId, movement.ProductVariantId, movement.LocationId, ct);
        if (level is null)
        {
            level = new StockLevel(movement.OrganizationId, movement.ProductVariantId, movement.LocationId);
            await _db.StockLevels.AddAsync(level, ct);
        }

        level.ApplyMovement(movement.SignedQuantity);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);

    public async Task<IReadOnlyList<StockMovement>> GetMovementsAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct)
    {
        return await _db.StockMovements
            .Where(x => x.OrganizationId == organizationId && x.ProductVariantId == variantId && x.LocationId == locationId)
            .OrderByDescending(x => x.OccurredAt)
            .ToListAsync(ct);
    }
}

public sealed class OrganizationRepository : IOrganizationRepository
{
    private readonly InventraDbContext _db;

    public OrganizationRepository(InventraDbContext db) => _db = db;

    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct) =>
        _db.Organizations.FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}

public sealed class AuditRepository : IAuditRepository
{
    private readonly InventraDbContext _db;

    public AuditRepository(InventraDbContext db) => _db = db;

    public async Task AddAsync(AuditLogEntry entry, CancellationToken ct) =>
        await _db.AuditLogEntries.AddAsync(entry, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
