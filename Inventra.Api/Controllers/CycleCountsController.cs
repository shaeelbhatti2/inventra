using Inventra.Application.CycleCounts;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/cycle-counts")]
public sealed class CycleCountsController : ControllerBase
{
    private readonly CycleCountService _service;

    public CycleCountsController(CycleCountService service) => _service = service;

    [HttpPost("{id:guid}/submit-line")]
    public async Task<IActionResult> SubmitLine(Guid id, [FromBody] SubmitCountLineRequest request, CancellationToken ct)
    {
        request.CycleCountId = id;
        await _service.SubmitLineAsync(request, ct);
        return NoContent();
    }
}
