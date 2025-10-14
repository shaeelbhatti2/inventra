using Inventra.Application.Products;
using Microsoft.AspNetCore.Mvc;

namespace Inventra.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ProductService _products;

    public ProductsController(ProductService products) => _products = products;

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] Guid organizationId, CancellationToken ct) =>
        Ok(await _products.ListAsync(organizationId, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct) =>
        Ok(await _products.CreateProductAsync(request, ct));
}
