using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FVD.PartsManager.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Artikel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Artikelnummer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Bezeichnung = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Beschreibung = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Lagerbestand = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Mindestbestand = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Artikel", x => x.Id);
                    table.CheckConstraint("CK_Artikel_Lagerbestand", "\"Lagerbestand\" >= 0");
                    table.CheckConstraint("CK_Artikel_Mindestbestand", "\"Mindestbestand\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "Lagerbewegungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ArtikelId = table.Column<int>(type: "integer", nullable: false),
                    Bewegungsart = table.Column<int>(type: "integer", nullable: false),
                    Menge = table.Column<int>(type: "integer", nullable: false),
                    Datum = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Bemerkung = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lagerbewegungen", x => x.Id);
                    table.CheckConstraint("CK_Lagerbewegung_Menge", "\"Menge\" > 0");
                    table.ForeignKey(
                        name: "FK_Lagerbewegungen_Artikel_ArtikelId",
                        column: x => x.ArtikelId,
                        principalTable: "Artikel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Artikel_Artikelnummer",
                table: "Artikel",
                column: "Artikelnummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lagerbewegungen_ArtikelId",
                table: "Lagerbewegungen",
                column: "ArtikelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lagerbewegungen");

            migrationBuilder.DropTable(
                name: "Artikel");
        }
    }
}
