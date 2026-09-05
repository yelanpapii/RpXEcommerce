using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Checkout.Infrastructure.Database.Migrations;
    /// <inheritdoc />
    public partial class InitialCheckout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "checkout");

            migrationBuilder.CreateTable(
                name: "checkouts",
                schema: "checkout",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    order_id = table.Column<string>(type: "text", nullable: false),
                    shipment_number = table.Column<string>(type: "text", nullable: true),
                    total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checkouts", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_checkouts_order_id",
                schema: "checkout",
                table: "checkouts",
                column: "order_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkouts",
                schema: "checkout");
    }
}
