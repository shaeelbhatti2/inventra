using Inventra.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventra.Infrastructure.Persistence.Migrations;

[DbContext(typeof(InventraDbContext))]
[Migration("20251008142200_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "organizations",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                BaseCurrency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                AllowBackorder = table.Column<bool>(type: "boolean", nullable: false),
                OverReceiptTolerancePercent = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_organizations", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "warehouses",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Code = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                TimeZoneId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                IsActive = table.Column<bool>(type: "boolean", nullable: false),
                OrganizationId = table.Column<Guid>(type: "uuid", nullable: false),
                address_line1 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                address_line2 = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                address_city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address_state = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                address_postal_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                address_country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_warehouses", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_warehouses_OrganizationId_Code",
            table: "warehouses",
            columns: new[] { "OrganizationId", "Code" },
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "warehouses");
        migrationBuilder.DropTable(name: "organizations");
    }
}
