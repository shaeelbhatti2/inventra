using Inventra.Application.Warehouses;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/warehouses")]
public sealed class WarehousesController : ControllerBase
{
    private readonly WarehouseService _warehouses;
    private readonly LocationService _locations;

    public WarehousesController(WarehouseService warehouses, LocationService locations)
    {
        _warehouses = warehouses;
        _locations = locations;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _warehouses.ListAsync(organizationId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseRequest request, CancellationToken ct) =>
        Ok(await _warehouses.CreateAsync(request, ct));

    [HttpPost("locations")]
    public async Task<IActionResult> CreateLocation([FromBody] CreateLocationRequest request, CancellationToken ct) =>
        Ok(await _locations.CreateAsync(request, ct));
}
