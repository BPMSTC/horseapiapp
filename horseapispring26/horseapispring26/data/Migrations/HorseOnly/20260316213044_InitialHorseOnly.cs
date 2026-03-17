using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace horseapispring26.data.Migrations.HorseOnly
{
    /// <inheritdoc />
    public partial class InitialHorseOnly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Horses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Gender = table.Column<short>(type: "smallint", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Sire = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Dam = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BreederName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PictureUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    TotalRacesRun = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Wins = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Places = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Shows = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CareerEarnings = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    CurrentOwner = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Trainer = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Horses_CareerEarnings",
                table: "Horses",
                column: "CareerEarnings");

            migrationBuilder.CreateIndex(
                name: "IX_Horses_Gender",
                table: "Horses",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Horses_RegistrationNumber",
                table: "Horses",
                column: "RegistrationNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Horses");
        }
    }
}
