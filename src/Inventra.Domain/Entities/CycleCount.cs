using Inventra.Domain.Common;
using Inventra.Domain.Enums;
using Inventra.Domain.Exceptions;

namespace Inventra.Domain.Entities;

public sealed class CycleCount : OrganizationScopedEntity
{
    public string Name { get; private set; } = string.Empty;

    public CycleCountStatus Status { get; private set; } = CycleCountStatus.Open;

    public bool BlindCount { get; private set; }

    public Guid? LocationRangeStartId { get; private set; }

    public Guid? LocationRangeEndId { get; private set; }

    public string? CategoryFilter { get; private set; }

    private readonly List<CycleCountLine> _lines = new();

    public IReadOnlyCollection<CycleCountLine> Lines => _lines.AsReadOnly();

    private CycleCount()
    {
    }

    public CycleCount(Guid organizationId, string name, bool blindCount)
        : base(organizationId)
    {
        SetName(name);
        BlindCount = blindCount;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Cycle count name is required.");
        }

        Name = name.Trim();
        Touch();
    }

    public void SetLocationRange(Guid startId, Guid endId)
    {
        LocationRangeStartId = startId;
        LocationRangeEndId = endId;
        Touch();
    }

    public void SetCategoryFilter(string? category)
    {
        CategoryFilter = category?.Trim();
        Touch();
    }

    public CycleCountLine AddLine(Guid productVariantId, Guid locationId, decimal expectedQty)
    {
        var line = new CycleCountLine(Id, productVariantId, locationId, expectedQty);
        _lines.Add(line);
        Touch();
        return line;
    }

    public void Start()
    {
        EnsureStatus(CycleCountStatus.Open);
        Status = CycleCountStatus.InProgress;
        Touch();
    }

    public void SubmitForApproval()
    {
        EnsureStatus(CycleCountStatus.InProgress);
        Status = CycleCountStatus.PendingApproval;
        Touch();
    }

    public void Complete()
    {
        Status = CycleCountStatus.Completed;
        Touch();
    }

    private void EnsureStatus(CycleCountStatus expected)
    {
        if (Status != expected)
        {
            throw new DomainException($"Expected cycle count status {expected} but was {Status}.");
        }
    }
}
