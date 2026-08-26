using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Lost_Found.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnik",
                columns: table => new
                {
                    korisnikID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    korisnickoIme = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    lozinka = table.Column<string>(type: "text", nullable: false),
                    vremeKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    tipKorisnika = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnik", x => x.korisnikID);
                    table.CheckConstraint("CK_Korisnik_TipKorisnika", "\"tipKorisnika\" IN (0, 1)");
                });

            migrationBuilder.CreateTable(
                name: "Oglas",
                columns: table => new
                {
                    oglasID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    naziv = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    opis = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    datumKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    tip = table.Column<int>(type: "integer", nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(10,8)", nullable: true),
                    longitude = table.Column<decimal>(type: "numeric(11,8)", nullable: true),
                    fotografija = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    kreatorID = table.Column<int>(type: "integer", nullable: false),
                    adminID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Oglas", x => x.oglasID);
                    table.CheckConstraint("CK_Oglas_Latitude", "\"latitude\" IS NULL OR (\"latitude\" >= -90 AND \"latitude\" <= 90)");
                    table.CheckConstraint("CK_Oglas_Longitude", "\"longitude\" IS NULL OR (\"longitude\" >= -180 AND \"longitude\" <= 180)");
                    table.CheckConstraint("CK_Oglas_Tip", "\"tip\" IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_Oglas_Korisnik_adminID",
                        column: x => x.adminID,
                        principalTable: "Korisnik",
                        principalColumn: "korisnikID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Oglas_Korisnik_kreatorID",
                        column: x => x.kreatorID,
                        principalTable: "Korisnik",
                        principalColumn: "korisnikID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Potrazivanje",
                columns: table => new
                {
                    korisnikID = table.Column<int>(type: "integer", nullable: false),
                    oglasID = table.Column<int>(type: "integer", nullable: false),
                    datumKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    status = table.Column<int>(type: "integer", nullable: false),
                    datumRazresavanja = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Potrazivanje", x => new { x.korisnikID, x.oglasID });
                    table.CheckConstraint("CK_Potrazivanje_Status", "\"status\" IN (0, 1, 2)");
                    table.ForeignKey(
                        name: "FK_Potrazivanje_Korisnik_korisnikID",
                        column: x => x.korisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "korisnikID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Potrazivanje_Oglas_oglasID",
                        column: x => x.oglasID,
                        principalTable: "Oglas",
                        principalColumn: "oglasID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Razgovor",
                columns: table => new
                {
                    razgovorID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datumKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    statusRazgovora = table.Column<int>(type: "integer", nullable: false),
                    oglasID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Razgovor", x => x.razgovorID);
                    table.CheckConstraint("CK_Razgovor_Status", "\"statusRazgovora\" IN (0, 1)");
                    table.ForeignKey(
                        name: "FK_Razgovor_Oglas_oglasID",
                        column: x => x.oglasID,
                        principalTable: "Oglas",
                        principalColumn: "oglasID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Poruka",
                columns: table => new
                {
                    porukaID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    korisnikID = table.Column<int>(type: "integer", nullable: false),
                    razgovorID = table.Column<int>(type: "integer", nullable: false),
                    datumKreiranja = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    sadrzaj = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poruka", x => x.porukaID);
                    table.ForeignKey(
                        name: "FK_Poruka_Korisnik_korisnikID",
                        column: x => x.korisnikID,
                        principalTable: "Korisnik",
                        principalColumn: "korisnikID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Poruka_Razgovor_razgovorID",
                        column: x => x.razgovorID,
                        principalTable: "Razgovor",
                        principalColumn: "razgovorID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Korisnik",
                columns: new[] { "korisnikID", "email", "korisnickoIme", "lozinka", "tipKorisnika", "vremeKreiranja" },
                values: new object[] { 1, "admin@lostfound.local", "admin", "$2a$11$dp3ZWPpuZlt46loF6AoALutXdQDkMvmCRkRm6Kk5UZKbIRbtFEd8a", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_email",
                table: "Korisnik",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Korisnik_korisnickoIme",
                table: "Korisnik",
                column: "korisnickoIme",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_adminID",
                table: "Oglas",
                column: "adminID");

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_kreatorID",
                table: "Oglas",
                column: "kreatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Oglas_tip",
                table: "Oglas",
                column: "tip");

            migrationBuilder.CreateIndex(
                name: "IX_Poruka_korisnikID",
                table: "Poruka",
                column: "korisnikID");

            migrationBuilder.CreateIndex(
                name: "IX_Poruka_razgovorID_datumKreiranja",
                table: "Poruka",
                columns: new[] { "razgovorID", "datumKreiranja" });

            migrationBuilder.CreateIndex(
                name: "IX_Potrazivanje_oglasID",
                table: "Potrazivanje",
                column: "oglasID");

            migrationBuilder.CreateIndex(
                name: "IX_Razgovor_oglasID",
                table: "Razgovor",
                column: "oglasID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Poruka");

            migrationBuilder.DropTable(
                name: "Potrazivanje");

            migrationBuilder.DropTable(
                name: "Razgovor");

            migrationBuilder.DropTable(
                name: "Oglas");

            migrationBuilder.DropTable(
                name: "Korisnik");
        }
    }
}
