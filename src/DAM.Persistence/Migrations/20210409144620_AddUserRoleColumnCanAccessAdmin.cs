using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddUserRoleColumnCanAccessAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanAccessAdmin",
                table: "UserRoles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanAccessAdmin",
                table: "UserRoles");
        }
    }
}
