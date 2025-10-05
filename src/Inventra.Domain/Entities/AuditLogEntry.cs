using Inventra.Domain.Common;

namespace Inventra.Domain.Entities;

public sealed class AuditLogEntry : OrganizationScopedEntity
{
    public string EntityType { get; private set; } = string.Empty;

    public Guid EntityId { get; private set; }

    public string Action { get; private set; } = string.Empty;

    public string? Details { get; private set; }

    public Guid? UserId { get; private set; }

    public DateTimeOffset OccurredAt { get; private set; }

    private AuditLogEntry()
    {
    }

    public AuditLogEntry(
        Guid organizationId,
        string entityType,
        Guid entityId,
        string action,
        string? details,
        Guid? userId)
        : base(organizationId)
    {
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Details = details;
        UserId = userId;
        OccurredAt = DateTimeOffset.UtcNow;
    }
}
