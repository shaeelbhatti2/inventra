using Inventra.Application.Abstractions;
using Inventra.Domain.Entities;
using Inventra.Domain.Exceptions;

namespace Inventra.Application.SerialNumbers;

public sealed class CaptureSerialRequest
{
    public Guid OrganizationId { get; init; }
    public Guid ProductVariantId { get; init; }
    public string Number { get; init; } = string.Empty;
    public Guid LocationId { get; init; }
}

public sealed class SerialNumberService
{
    private readonly ISerialNumberRepository _repository;

    public SerialNumberService(ISerialNumberRepository repository)
    {
        _repository = repository;
    }

    public async Task<SerialNumber> CaptureOnReceiptAsync(CaptureSerialRequest request, CancellationToken ct)
    {
        var existing = await _repository.GetByNumberAsync(request.OrganizationId, request.Number, ct);
        if (existing is not null)
        {
            throw new DomainException("Serial number already exists.");
        }

        var serial = new SerialNumber(
            request.OrganizationId,
            request.ProductVariantId,
            request.Number,
            request.LocationId);

        await _repository.AddAsync(serial, ct);
        await _repository.SaveChangesAsync(ct);
        return serial;
    }

    public async Task ValidateAndShipAsync(Guid organizationId, string number, CancellationToken ct)
    {
        var serial = await _repository.GetByNumberAsync(organizationId, number, ct)
            ?? throw new DomainException("Serial not found.");

        if (serial.IsShipped)
        {
            throw new DomainException("Serial already shipped.");
        }

        serial.MarkShipped();
        await _repository.SaveChangesAsync(ct);
    }

    public Task<SerialNumber?> LookupAsync(Guid organizationId, string number, CancellationToken ct) =>
        _repository.GetByNumberAsync(organizationId, number, ct);
}
