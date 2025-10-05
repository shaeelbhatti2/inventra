using Inventra.Domain.Common;
using Inventra.Domain.Exceptions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Domain.Entities;

public sealed class Warehouse : OrganizationScopedEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public Address Address { get; private set; }

    public string TimeZoneId { get; private set; } = "UTC";

    public bool IsActive { get; private set; } = true;

    private Warehouse()
    {
        Address = new Address(string.Empty, null, string.Empty, string.Empty, string.Empty, string.Empty);
    }

    public Warehouse(Guid organizationId, string code, string name, Address address, string timeZoneId)
        : base(organizationId)
    {
        SetCode(code);
        SetName(name);
        Address = address;
        SetTimeZoneId(timeZoneId);
    }

    public void SetCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new DomainException("Warehouse code is required.");
        }

        Code = code.Trim().ToUpperInvariant();
        Touch();
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Warehouse name is required.");
        }

        Name = name.Trim();
        Touch();
    }

    public void SetAddress(Address address)
    {
        Address = address;
        Touch();
    }

    public void SetTimeZoneId(string timeZoneId)
    {
        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            throw new DomainException("Time zone is required.");
        }

        TimeZoneId = timeZoneId.Trim();
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
