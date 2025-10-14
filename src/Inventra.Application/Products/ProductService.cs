using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Products;

public sealed class CreateProductRequest
{
    public Guid OrganizationId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? Category { get; init; }
}

public sealed class CreateVariantRequest
{
    public Guid OrganizationId { get; init; }
    public Guid ProductId { get; init; }
    public string Sku { get; init; } = string.Empty;
    public string? Size { get; init; }
    public string? Color { get; init; }
    public UnitOfMeasure UnitOfMeasure { get; init; } = UnitOfMeasure.Each;
    public decimal ReorderPoint { get; init; }
    public decimal ReorderQuantity { get; init; }
}

public sealed class ProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request, CancellationToken ct)
    {
        var existing = await _repository.GetVariantBySkuAsync(request.OrganizationId, request.Sku, ct);
        if (existing is not null)
        {
            throw new DomainException("SKU already exists.");
        }

        var product = new Product(request.OrganizationId, request.Sku, request.Name);
        product.SetDescription(request.Description);
        product.SetCategory(request.Category);

        await _repository.AddAsync(product, ct);
        await _repository.SaveChangesAsync(ct);
        return product;
    }

    public async Task<ProductVariant> CreateVariantAsync(CreateVariantRequest request, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(request.OrganizationId, request.ProductId, ct)
            ?? throw new DomainException("Product not found.");

        if (product.IsDeleted)
        {
            throw new DomainException("Product is deleted.");
        }

        var variant = new ProductVariant(request.OrganizationId, request.ProductId, request.Sku);
        variant.SetAttributes(request.Size, request.Color);
        variant.SetUnitOfMeasure(request.UnitOfMeasure);
        variant.SetReorderLevels(new Quantity(request.ReorderPoint), new Quantity(request.ReorderQuantity));

        await _repository.AddVariantAsync(variant, ct);
        await _repository.SaveChangesAsync(ct);
        return variant;
    }

    public async Task SoftDeleteProductAsync(Guid organizationId, Guid productId, CancellationToken ct)
    {
        var product = await _repository.GetByIdAsync(organizationId, productId, ct)
            ?? throw new DomainException("Product not found.");

        product.SoftDelete();
        await _repository.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<Product>> ListAsync(Guid organizationId, CancellationToken ct) =>
        _repository.ListAsync(organizationId, ct);
}
