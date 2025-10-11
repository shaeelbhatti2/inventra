using FluentAssertions;
using Inventra.Application.Abstractions;
using Inventra.Application.StockLedger;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Tests;

public class StockLedgerServiceTests
{
    [Fact]
    public async Task RecordMovement_rejects_negative_stock_without_backorder()
    {
        var orgId = Guid.NewGuid();
        var org = new Organization("Acme");
        typeof(Organization).GetProperty(nameof(Organization.Id))!.SetValue(org, orgId);

        var repo = new FakeStockRepo();
        var service = new StockLedgerService(repo, new FakeOrgRepo(org), new FakeAuditRepo());

        var act = () => service.RecordMovementAsync(new RecordMovementRequest
        {
            OrganizationId = orgId,
            ProductVariantId = Guid.NewGuid(),
            LocationId = Guid.NewGuid(),
            Type = MovementType.Shipment,
            Quantity = 5m
        }, CancellationToken.None);

        await act.Should().ThrowAsync<DomainException>();
    }
}

internal sealed class FakeStockRepo : IStockLedgerRepository
{
    public Task<StockLevel?> GetLevelAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct) =>
        Task.FromResult<StockLevel?>(null);

    public Task AddMovementAsync(StockMovement movement, CancellationToken ct) => Task.CompletedTask;
    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
    public Task<IReadOnlyList<StockMovement>> GetMovementsAsync(Guid organizationId, Guid variantId, Guid locationId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<StockMovement>>(Array.Empty<StockMovement>());
}

internal sealed class FakeOrgRepo : IOrganizationRepository
{
    private readonly Organization _org;
    public FakeOrgRepo(Organization org) => _org = org;
    public Task<Organization?> GetByIdAsync(Guid id, CancellationToken ct) => Task.FromResult<Organization?>(_org);
    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
}

internal sealed class FakeAuditRepo : IAuditRepository
{
    public Task AddAsync(AuditLogEntry entry, CancellationToken ct) => Task.CompletedTask;
    public Task SaveChangesAsync(CancellationToken ct) => Task.CompletedTask;
}
