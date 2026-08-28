using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodajOpisLokacijeOglas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OpisLokacije",
                table: "Oglas",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OpisLokacije",
                table: "Oglas");
        }
    }
}
