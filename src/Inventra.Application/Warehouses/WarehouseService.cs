using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Warehouses;

public sealed class CreateWarehouseRequest
{
    public Guid OrganizationId { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Address Address { get; init; }
    public string TimeZoneId { get; init; } = "UTC";
}

public sealed class CreateLocationRequest
{
    public Guid OrganizationId { get; init; }
    public Guid WarehouseId { get; init; }
    public LocationType Type { get; init; }
    public string Code { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public Guid? ParentLocationId { get; init; }
    public decimal? MaxUnits { get; init; }
    public decimal? MaxWeightKg { get; init; }
}

public sealed class WarehouseService
{
    private readonly IWarehouseRepository _repository;

    public WarehouseService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Warehouse> CreateAsync(CreateWarehouseRequest request, CancellationToken ct)
    {
        var warehouse = new Warehouse(
            request.OrganizationId,
            request.Code,
            request.Name,
            request.Address,
            request.TimeZoneId);

        await _repository.AddAsync(warehouse, ct);
        await _repository.SaveChangesAsync(ct);
        return warehouse;
    }

    public Task<IReadOnlyList<Warehouse>> ListAsync(Guid organizationId, CancellationToken ct) =>
        _repository.ListAsync(organizationId, ct);

    public Task<Warehouse?> GetAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _repository.GetByIdAsync(organizationId, id, ct);
}

public sealed class LocationService
{
    private readonly IWarehouseRepository _repository;

    public LocationService(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Location> CreateAsync(CreateLocationRequest request, CancellationToken ct)
    {
        var warehouse = await _repository.GetByIdAsync(request.OrganizationId, request.WarehouseId, ct)
            ?? throw new DomainException("Warehouse not found.");

        if (!warehouse.IsActive)
        {
            throw new DomainException("Warehouse is inactive.");
        }

        var location = new Location(
            request.OrganizationId,
            request.WarehouseId,
            request.Type,
            request.Code,
            request.Name,
            request.ParentLocationId);

        location.SetCapacity(request.MaxUnits, request.MaxWeightKg);
        location.AssignBarcode($"LOC-{request.Code}");

        await _repository.AddLocationAsync(location, ct);
        await _repository.SaveChangesAsync(ct);
        return location;
    }

    public async Task DeactivateAsync(Guid organizationId, Guid locationId, CancellationToken ct)
    {
        var location = await _repository.GetLocationByIdAsync(organizationId, locationId, ct)
            ?? throw new DomainException("Location not found.");

        location.Deactivate();
        await _repository.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<Location>> ListByWarehouseAsync(Guid organizationId, Guid warehouseId, CancellationToken ct) =>
        _repository.ListLocationsAsync(organizationId, warehouseId, ct);
}
