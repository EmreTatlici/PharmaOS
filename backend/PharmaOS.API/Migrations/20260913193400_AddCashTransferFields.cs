using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaOS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCashTransferFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CashDifference",
                table: "DailyClosings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ClosingCashAmount",
                table: "DailyClosings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OpeningCashAmount",
                table: "DailyClosings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CashDifference",
                table: "DailyClosings");

            migrationBuilder.DropColumn(
                name: "ClosingCashAmount",
                table: "DailyClosings");

            migrationBuilder.DropColumn(
                name: "OpeningCashAmount",
                table: "DailyClosings");
        }
    }
}
