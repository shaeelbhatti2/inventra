# Inventra

Self-hosted warehouse and inventory management platform built on .NET 8.

Inventra gives small warehouses, e-commerce ops, and distributors a single source of truth for stock levels, locations, purchase orders, and fulfillment. Staff receive goods, pick orders, transfer between bins, and run cycle counts with full audit trails.

## Stack

- C# 12 / .NET 8
- ASP.NET Core Minimal APIs + Blazor Server admin UI
- PostgreSQL + EF Core 8
- MediatR for application commands
- Hangfire for background alerts
- JWT auth for scanner devices

## Architecture

```
src/
  Inventra.Domain/          entities, value objects, domain rules
  Inventra.Application/     CQRS handlers, services
  Inventra.Infrastructure/    EF Core, repositories, identity
  Inventra.Api/               REST + barcode scanning endpoints
  Inventra.Web/               Blazor Server admin UI
  Inventra.Jobs/              background workers
tests/
  Inventra.Domain.Tests/
  Inventra.Application.Tests/
  Inventra.Integration.Tests/
```

Stock changes always flow through an append-only **StockMovement** ledger. On-hand quantity is derived from movement sums per product and location — quantities are never updated without a corresponding movement record.

### Movement types

| Type | Description |
|------|-------------|
| RECEIPT | Inbound from purchase order |
| SHIPMENT | Outbound from sales order |
| TRANSFER_OUT | Leave source location |
| TRANSFER_IN | Arrive at destination |
| ADJUSTMENT | Cycle count variance |
| CYCLE_COUNT | Count submission |

## Local setup

1. Install [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Copy environment file:

```bash
cp .env.example .env
```

3. Start PostgreSQL and Redis:

```bash
docker compose up -d
```

4. Run migrations (once available):

```bash
dotnet ef database update --project src/Inventra.Infrastructure --startup-project src/Inventra.Api
```

5. Start the API:

```bash
dotnet run --project src/Inventra.Api
```

6. Start the Blazor admin UI:

```bash
dotnet run --project src/Inventra.Web
```

API health check: `http://localhost:5000/health`

## Barcode scanning workflow

Scanner devices authenticate via JWT and call low-latency endpoints:

- `GET /api/products/by-barcode/{code}` — resolve SKU
- `POST /api/movements/receive` — scan PO line + qty
- `POST /api/movements/pick` — confirm pick on SO line
- `POST /api/cycle-counts/{id}/submit-line` — blind count entry

## Cycle count walkthrough

1. Manager creates a cycle count for a location range or category
2. Counter submits blind quantities via scanner or UI
3. System computes variance against ledger balance
4. Manager approves variances → ADJUSTMENT movements posted

## Reports

- Stock on hand by warehouse/location
- Movement history with audit trail
- Inventory valuation (FIFO weighted average on receipt cost)
- Lot trace and expiry reports

## Future improvements

- Multi-org SaaS tenancy
- ERP webhook integrations
- Mobile offline-first scanner app
- Advanced demand forecasting

## License

MIT — see [LICENSE](LICENSE).
