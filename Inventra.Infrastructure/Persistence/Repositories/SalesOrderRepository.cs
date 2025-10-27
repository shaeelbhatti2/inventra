using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class SalesOrderRepository : ISalesOrderRepository
{
    private readonly InventraDbContext _db;

    public SalesOrderRepository(InventraDbContext db) => _db = db;

    public Task<SalesOrder?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.SalesOrders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task<IReadOnlyList<SalesOrder>> ListAsync(Guid organizationId, CancellationToken ct) =>
        await _db.SalesOrders
            .Where(x => x.OrganizationId == organizationId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(ct);

    public async Task AddAsync(SalesOrder order, CancellationToken ct) =>
        await _db.SalesOrders.AddAsync(order, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
