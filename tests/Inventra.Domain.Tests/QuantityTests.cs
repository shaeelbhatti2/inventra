using FluentAssertions;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Tests;

public class QuantityTests
{
    [Fact]
    public void Add_combines_values()
    {
        var result = new Quantity(3m) + new Quantity(2.5m);
        result.Value.Should().Be(5.5m);
    }

    [Fact]
    public void EnsureNonNegative_throws_for_negative()
    {
        var qty = new Quantity(-1m);
        var act = () => qty.EnsureNonNegative();
        act.Should().Throw<DomainException>();
    }
}

public class MoneyTests
{
    [Fact]
    public void Add_requires_same_currency()
    {
        var left = new Money(10m, "USD");
        var right = new Money(5m, "EUR");
        var act = () => left + right;
        act.Should().Throw<DomainException>();
    }
}
