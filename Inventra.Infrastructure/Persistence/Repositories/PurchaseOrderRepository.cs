using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly InventraDbContext _db;

    public PurchaseOrderRepository(InventraDbContext db) => _db = db;

    public Task<PurchaseOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.PurchaseOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task<IReadOnlyList<PurchaseOrder>> ListAsync(Guid organizationId, CancellationToken ct) =>
        await _db.PurchaseOrders
            .Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(PurchaseOrder order, CancellationToken ct) =>
        await _db.PurchaseOrders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
