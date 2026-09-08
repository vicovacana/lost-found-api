using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    /// <inheritdoc />
    public partial class DodajProcitaliKorisniciNaPoruku : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<int>>(
                name: "procitaliKorisnici",
                table: "Poruka",
                type: "integer[]",
                nullable: false,
                defaultValueSql: "'{}'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "procitaliKorisnici",
                table: "Poruka");
        }
    }
}
