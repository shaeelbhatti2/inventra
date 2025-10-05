using Inventra.Domain.Exceptions;

namespace Inventra.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }

    public string Currency { get; }

    public Money(decimal amount, string currency = "USD")
    {
        if (string.IsNullOrWhiteSpace(currency))
        {
            throw new DomainException("Currency is required.");
        }

        Amount = amount;
        Currency = currency.Trim().ToUpperInvariant();
    }

    public Money Add(Money other) => this + other;

    public Money Subtract(Money other) => this - other;

    public Money Round(int decimals = 2) =>
        new(decimal.Round(Amount, decimals, MidpointRounding.ToEven), Currency);

    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return new Money(left.Amount - right.Amount, left.Currency);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
        {
            throw new DomainException("Cannot combine amounts with different currencies.");
        }
    }
}
