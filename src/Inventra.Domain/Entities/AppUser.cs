using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class AppUser : OrganizationScopedEntity
{
    public string Email { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public UserRole Role { get; private set; }

    public bool IsActive { get; private set; } = true;

    private AppUser()
    {
    }

    public AppUser(Guid organizationId, string email, string displayName, UserRole role)
        : base(organizationId)
    {
        SetEmail(email);
        SetDisplayName(displayName);
        Role = role;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email is required.");
        }

        Email = email.Trim().ToLowerInvariant();
        Touch();
    }

    public void SetDisplayName(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new DomainException("Display name is required.");
        }

        DisplayName = displayName.Trim();
        Touch();
    }

    public void SetRole(UserRole role)
    {
        Role = role;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }
}
