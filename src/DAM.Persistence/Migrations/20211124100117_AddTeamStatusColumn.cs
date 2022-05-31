using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddTeamStatusColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Teams",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Teams");
        }
    }
}
