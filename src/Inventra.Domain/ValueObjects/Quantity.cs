using Inventra.Domain.Exceptions;

namespace Inventra.Domain.ValueObjects;

public readonly record struct Quantity
{
    public decimal Value { get; }

    public Quantity(decimal value)
    {
        Value = value;
    }

    public static Quantity Zero => new(0m);

    public Quantity Add(Quantity other) => this + other;

    public Quantity Subtract(Quantity other) => this - other;

    public bool IsNegative => Value < 0m;

    public bool IsZero => Value == 0m;

    public Quantity Abs() => new(Math.Abs(Value));

    public static Quantity operator +(Quantity left, Quantity right) =>
        new(left.Value + right.Value);

    public static Quantity operator -(Quantity left, Quantity right) =>
        new(left.Value - right.Value);

    public static Quantity operator *(Quantity left, decimal multiplier) =>
        new(left.Value * multiplier);

    public static bool operator <(Quantity left, Quantity right) =>
        left.Value < right.Value;

    public static bool operator >(Quantity left, Quantity right) =>
        left.Value > right.Value;

    public static bool operator <=(Quantity left, Quantity right) =>
        left.Value <= right.Value;

    public static bool operator >=(Quantity left, Quantity right) =>
        left.Value >= right.Value;

    public void EnsureNonNegative()
    {
        if (IsNegative)
        {
            throw new DomainException("Quantity cannot be negative.");
        }
    }
}
