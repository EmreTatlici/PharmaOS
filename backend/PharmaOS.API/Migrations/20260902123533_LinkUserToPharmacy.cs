using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaOS.API.Migrations
{
    /// <inheritdoc />
    public partial class LinkUserToPharmacy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PharmacyId",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Users_PharmacyId",
                table: "Users",
                column: "PharmacyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Pharmacies_PharmacyId",
                table: "Users",
                column: "PharmacyId",
                principalTable: "Pharmacies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Pharmacies_PharmacyId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_PharmacyId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "Users");
        }
    }
}
