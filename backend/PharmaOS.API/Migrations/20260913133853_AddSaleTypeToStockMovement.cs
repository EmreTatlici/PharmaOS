using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaOS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSaleTypeToStockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaleType",
                table: "StockMovements",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleType",
                table: "StockMovements");
        }
    }
}
