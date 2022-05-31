using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddDeletedAndPrevStatusBeforeArchiveColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Projects",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "StatusBeforeArchive",
                table: "Projects",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "StatusBeforeArchive",
                table: "Projects");

     
        }
    }
}
