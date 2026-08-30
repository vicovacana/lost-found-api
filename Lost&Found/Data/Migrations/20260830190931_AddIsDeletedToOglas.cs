using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToOglas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "obrisan",
                table: "Oglas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_obrisan",
                table: "Oglas",
                column: "obrisan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oglas_obrisan",
                table: "Oglas");

            migrationBuilder.DropColumn(
                name: "obrisan",
                table: "Oglas");
        }
    }
}
