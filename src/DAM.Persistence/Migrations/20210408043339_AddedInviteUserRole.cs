using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddedInviteUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanInvite",
                table: "UserRoles",
                nullable: false,
                defaultValue: false);

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
                columns: new[] { "CanInvite", "RoleName" },
                values: new object[] { true, "Dubber Admin" });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CanAdd", "CanArchive", "CanDelete", "CanEdit", "CanInvite", "CanMove", "CanShare", "CanUpload", "RoleName" },
                values: new object[] { false, false, false, false, true, false, false, false, "Company Admin" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanInvite",
                table: "UserRoles");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 4, 7, 10, 52, 10, 424, DateTimeKind.Unspecified).AddTicks(54), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1,
                column: "RoleName",
                value: "Administrator");

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CanAdd", "CanArchive", "CanDelete", "CanEdit", "CanMove", "CanShare", "CanUpload", "RoleName" },
                values: new object[] { true, true, true, true, true, true, true, "Partner Admin" });
        }
    }
}
