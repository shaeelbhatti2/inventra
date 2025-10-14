using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventra.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly InventraDbContext _db;

    public ProductRepository(InventraDbContext db) => _db = db;

    public Task<Product?> GetByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.Products.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id && !x.IsDeleted, ct);

    public Task<ProductVariant?> GetVariantByIdAsync(Guid organizationId, Guid id, CancellationToken ct) =>
        _db.ProductVariants.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Id == id, ct);

    public Task<ProductVariant?> GetVariantBySkuAsync(Guid organizationId, string sku, CancellationToken ct) =>
        _db.ProductVariants.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Sku == sku.ToUpperInvariant(), ct);

    public Task<ProductVariant?> GetVariantByBarcodeAsync(Guid organizationId, string barcode, CancellationToken ct) =>
        _db.ProductVariants.FirstOrDefaultAsync(x => x.OrganizationId == organizationId && x.Barcode == barcode, ct);

    public async Task<IReadOnlyList<Product>> ListAsync(Guid organizationId, CancellationToken ct) =>
        await _db.Products.Where(x => x.OrganizationId == organizationId && !x.IsDeleted).OrderBy(x => x.Sku).ToListAsync(ct);

    public async Task AddAsync(Product product, CancellationToken ct) =>
        await _db.Products.AddAsync(product, ct);

    public async Task AddVariantAsync(ProductVariant variant, CancellationToken ct) =>
        await _db.ProductVariants.AddAsync(variant, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
