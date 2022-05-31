using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class RenameRoleName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleName",
                table: "UserRoles");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanDelete",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanInvite",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanMove",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CanUpload",
                table: "AspNetRoles",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 8, 13, 17, 54, 737, DateTimeKind.Unspecified).AddTicks(2062), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Dubber Admin");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Company Admin");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Subscriber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "CanDelete",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CanInvite",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CanMove",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "CanUpload",
                table: "AspNetRoles");

            migrationBuilder.AddColumn<string>(
                name: "RoleName",
                table: "UserRoles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 8, 4, 33, 38, 605, DateTimeKind.Unspecified).AddTicks(8518), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleName",
                value: "Dubber Admin");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                column: "RoleName",
                value: "Company Admin");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3,
                column: "RoleName",
                value: "Subscriber");
        }
    }
}
