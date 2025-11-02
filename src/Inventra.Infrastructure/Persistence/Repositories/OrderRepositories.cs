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
