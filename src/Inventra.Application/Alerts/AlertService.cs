using Inventra.Application.Abstractions;
using Inventra.Domain.ValueObjects;

namespace Inventra.Application.Alerts;

public sealed class AlertService
{
    private readonly IAlertRepository _alerts;
    private readonly IProductRepository _products;
    private readonly IStockLedgerRepository _stock;

    public AlertService(IAlertRepository alerts, IProductRepository products, IStockLedgerRepository stock)
    {
        _alerts = alerts;
        _products = products;
        _stock = stock;
    }

    public async Task RunLowStockCheckAsync(Guid organizationId, CancellationToken ct)
    {
        var products = await _products.ListAsync(organizationId, ct);
        foreach (var product in products)
        {
            var variant = await _products.GetVariantBySkuAsync(organizationId, product.Sku, ct);
            if (variant is null || variant.ReorderPoint.IsZero)
            {
                continue;
            }

            var onHand = await _stock.GetLevelAsync(organizationId, variant.Id, Guid.Empty, ct);
            var qty = onHand?.OnHand ?? Quantity.Zero;
            if (qty > variant.ReorderPoint)
            {
                continue;
            }

            var key = $"low-stock:{variant.Id}";
            if (await _alerts.ExistsAsync(organizationId, key, ct))
            {
                continue;
            }

            await _alerts.AddAsync(new StockAlert
            {
                OrganizationId = organizationId,
                AlertKey = key,
                Message = $"{variant.Sku} below reorder point ({qty.Value} on hand)"
            }, ct);
        }

        await _alerts.SaveChangesAsync(ct);
    }

    public Task<IReadOnlyList<StockAlert>> ListOpenAsync(Guid organizationId, CancellationToken ct) =>
        _alerts.ListOpenAsync(organizationId, ct);
}
