using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class Organization : Entity
{
    public string Name { get; private set; } = string.Empty;

    public string BaseCurrency { get; private set; } = "USD";

    public bool AllowBackorder { get; private set; }

    public decimal OverReceiptTolerancePercent { get; private set; }

    private Organization()
    {
    }

    public Organization(string name, string baseCurrency = "USD")
    {
        SetName(name);
        BaseCurrency = baseCurrency.Trim().ToUpperInvariant();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Organization name is required.");
        }

        Name = name.Trim();
        Touch();
    }

    public void SetAllowBackorder(bool allow)
    {
        AllowBackorder = allow;
        Touch();
    }

    public void SetOverReceiptTolerancePercent(decimal percent)
    {
        if (percent < 0m)
        {
            throw new DomainException("Over-receipt tolerance cannot be negative.");
        }

        OverReceiptTolerancePercent = percent;
        Touch();
    }
}
