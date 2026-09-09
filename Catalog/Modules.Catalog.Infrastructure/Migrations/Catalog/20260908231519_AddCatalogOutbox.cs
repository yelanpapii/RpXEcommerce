using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable IDE0161, MA0051, CA1861

namespace Modules.Catalog.Infrastructure.Migrations.Catalog;

    /// <inheritdoc />
    public partial class AddCatalogOutbox : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "sku",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.AddColumn<string>(
                name: "category_code",
                schema: "catalog",
                table: "catalog_products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "style_id",
                schema: "catalog",
                table: "catalog_products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "catalog_product_variants",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sku = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    color_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    size_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    stock = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_catalog_product_variants", x => x.id);
                    table.ForeignKey(
                        name: "fk_catalog_product_variants_catalog_products_product_id",
                        column: x => x.product_id,
                        principalSchema: "catalog",
                        principalTable: "catalog_products",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "catalog",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    occurred_on_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    locked_until_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    attempt_count = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_catalog_product_variants_product_id",
                schema: "catalog",
                table: "catalog_product_variants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_product_variants_sku",
                schema: "catalog",
                table: "catalog_product_variants",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_on_utc_occurred_on_utc",
                schema: "catalog",
                table: "outbox_messages",
                columns: new[] { "processed_on_utc", "occurred_on_utc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "catalog_product_variants",
                schema: "catalog");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "catalog");

            migrationBuilder.DropColumn(
                name: "category_code",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "style_id",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.AddColumn<decimal>(
                name: "price",
                schema: "catalog",
                table: "catalog_products",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "sku",
                schema: "catalog",
                table: "catalog_products",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }
    }
#pragma warning restore IDE0161, MA0051, CA1861
