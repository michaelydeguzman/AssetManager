using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class ChangeAltToTertiaryColorColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltColor",
                table: "Styles");

            migrationBuilder.AddColumn<string>(
                name: "TertiaryColor",
                table: "Styles",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "TertiaryColor",
                value: "#00aa55");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TertiaryColor",
                table: "Styles");

            migrationBuilder.AddColumn<string>(
                name: "AltColor",
                table: "Styles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                column: "AltColor",
                value: "#00aa55");
        }
    }
}
