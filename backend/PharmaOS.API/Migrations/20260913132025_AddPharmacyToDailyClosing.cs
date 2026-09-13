using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmaOS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddPharmacyToDailyClosing : Migration
    {
        /// <inheritdoc />
protected override void Up(MigrationBuilder migrationBuilder)
{
    migrationBuilder.AddColumn<int>(
        name: "PharmacyId",
        table: "DailyClosings",
        type: "integer",
        nullable: false,
        defaultValue: 0);

    migrationBuilder.Sql("""
        UPDATE "DailyClosings"
        SET "PharmacyId" = 1
        WHERE "PharmacyId" = 0;
        """);

    migrationBuilder.CreateIndex(
        name: "IX_DailyClosings_PharmacyId",
        table: "DailyClosings",
        column: "PharmacyId");

    migrationBuilder.AddForeignKey(
        name: "FK_DailyClosings_Pharmacies_PharmacyId",
        table: "DailyClosings",
        column: "PharmacyId",
        principalTable: "Pharmacies",
        principalColumn: "Id",
        onDelete: ReferentialAction.Cascade);
}
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyClosings_Pharmacies_PharmacyId",
                table: "DailyClosings");

            migrationBuilder.DropIndex(
                name: "IX_DailyClosings_PharmacyId",
                table: "DailyClosings");

            migrationBuilder.DropColumn(
                name: "PharmacyId",
                table: "DailyClosings");
        }
    }
}
