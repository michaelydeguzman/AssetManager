using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class AddNewUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 5, 27, 7, 28, 19, 452, DateTimeKind.Unspecified).AddTicks(9960), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CanAccessAdmin", "CanAdd", "CanApprove", "CanArchive", "CanDelete", "CanEdit", "CanInvite", "CanMove", "CanShare", "CanUpload", "Name" },
                values: new object[] { 4, false, false, false, false, false, false, false, false, true, false, "User" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 5, 25, 7, 33, 18, 372, DateTimeKind.Unspecified).AddTicks(2061), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
