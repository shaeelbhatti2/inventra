using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class WarehouseRepository : IWarehouseRepository
{
    private readonly InventraDbContext _db;

    public WarehouseRepository(InventraDbContext db) => _db = db;

    public Task<Warehouse?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.Warehouses.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task<IReadOnlyList<Warehouse>> ListAsync(Guid organizationId, CancellationToken ct) =>
        await _db.Warehouses.Where(x => x.OrganizationId == organizationId).OrderBy(x => x.Code).ToListAsync(ct);

    public async Task AddAsync(Warehouse warehouse, CancellationToken ct) =>
        await _db.Warehouses.AddAsync(warehouse, ct);

    public Task<Location?> GetLocationByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.Locations.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public async Task<IReadOnlyList<Location>> ListLocationsAsync(Guid organizationId, Guid warehouseId, CancellationToken ct) =>
        await _db.Locations
            .Where(x => x.OrganizationId == organizationId && x.WarehouseId == warehouseId)
            .OrderBy(x => x.Code)
            .ToListAsync(ct);

    public async Task AddLocationAsync(Location location, CancellationToken ct) =>
        await _db.Locations.AddAsync(location, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
