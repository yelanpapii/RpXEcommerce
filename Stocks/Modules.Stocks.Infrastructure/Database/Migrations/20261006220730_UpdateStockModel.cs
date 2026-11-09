using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Stocks.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class UpdateStockModel : Migration
{
	/// <inheritdoc />
	protected override void Up(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.AddColumn<Guid>(
			name: "product_id",
			schema: "stocks",
			table: "product_stocks",
			type: "uuid",
			nullable: false,
			defaultValue: Guid.Empty);

		migrationBuilder.AddColumn<Guid>(
			name: "variant_id",
			schema: "stocks",
			table: "product_stocks",
			type: "uuid",
			nullable: false,
			defaultValue: Guid.Empty);
	}

	/// <inheritdoc />
	protected override void Down(MigrationBuilder migrationBuilder)
	{
		migrationBuilder.DropColumn(
			name: "product_id",
			schema: "stocks",
			table: "product_stocks");

		migrationBuilder.DropColumn(
			name: "variant_id",
			schema: "stocks",
			table: "product_stocks");
	}
}
