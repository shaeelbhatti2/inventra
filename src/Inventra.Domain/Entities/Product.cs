using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class Product : OrganizationScopedEntity
{
    public string Sku { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public string? Category { get; private set; }

    public bool IsDeleted { get; private set; }

    private Product()
    {
    }

    public Product(Guid organizationId, string sku, string name)
        : base(organizationId)
    {
        SetSku(sku);
        SetName(name);
    }

    public void SetSku(string sku)
    {
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new DomainException("SKU is required.");
        }

        Sku = sku.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Product name is required.");
        }

        Name = name.Trim();
        Touch();
    }

    public void SetDescription(string? description)
    {
        Description = description?.Trim();
        Touch();
    }

    public void SetCategory(string? category)
    {
        Category = category?.Trim();
        Touch();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Touch();
    }
}
