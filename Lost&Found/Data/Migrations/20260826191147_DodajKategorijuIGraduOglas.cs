using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    public partial class DodajKategorijuIGraduOglas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "grad",
                table: "Oglas",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "kategorija",
                table: "Oglas",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_grad",
                table: "Oglas",
                column: "grad");

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_kategorija",
                table: "Oglas",
                column: "kategorija");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Oglas_grad",
                table: "Oglas");

            migrationBuilder.DropIndex(
                name: "IX_Oglas_kategorija",
                table: "Oglas");

            migrationBuilder.DropColumn(
                name: "grad",
                table: "Oglas");

            migrationBuilder.DropColumn(
                name: "kategorija",
                table: "Oglas");
        }
    }
}
