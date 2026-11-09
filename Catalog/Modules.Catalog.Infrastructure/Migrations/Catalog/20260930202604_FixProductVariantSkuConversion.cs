using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Catalog.Infrastructure.Migrations.Catalog;

    /// <inheritdoc />
    public partial class FixProductVariantSkuConversion : Migration
    {
	/// <inheritdoc />
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "<pendiente>")]
	protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "color_name",
                schema: "catalog",
                table: "catalog_product_variants");

            migrationBuilder.DropColumn(
                name: "size_name",
                schema: "catalog",
                table: "catalog_product_variants");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "catalog",
                table: "catalog_products",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "category_code",
                schema: "catalog",
                table: "catalog_products",
                type: "character varying(8)",
                maxLength: 8,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "status",
                schema: "catalog",
                table: "catalog_products",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "updated_at",
                schema: "catalog",
                table: "catalog_products",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "color_code",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(6)",
                maxLength: 6,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(3)",
                oldMaxLength: 3);

            migrationBuilder.AddColumn<byte[]>(
                name: "row_version",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                defaultValue: Array.Empty<byte>());

            migrationBuilder.AddColumn<string>(
                name: "size",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_catalog_products_category_code_style_id",
                schema: "catalog",
                table: "catalog_products",
                columns: ["category_code", "style_id"],
                unique: true);
        }

	/// <inheritdoc />
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0051:Method is too long", Justification = "<pendiente>")]
	protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_catalog_products_category_code_style_id",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "status",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "catalog",
                table: "catalog_products");

            migrationBuilder.DropColumn(
                name: "row_version",
                schema: "catalog",
                table: "catalog_product_variants");

            migrationBuilder.DropColumn(
                name: "size",
                schema: "catalog",
                table: "catalog_product_variants");

            migrationBuilder.AlterColumn<string>(
                name: "description",
                schema: "catalog",
                table: "catalog_products",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "category_code",
                schema: "catalog",
                table: "catalog_products",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(8)",
                oldMaxLength: 8);

            migrationBuilder.AlterColumn<string>(
                name: "sku",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(40)",
                oldMaxLength: 40);

            migrationBuilder.AlterColumn<string>(
                name: "color_code",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(6)",
                oldMaxLength: 6);

            migrationBuilder.AddColumn<string>(
                name: "color_name",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "size_name",
                schema: "catalog",
                table: "catalog_product_variants",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
