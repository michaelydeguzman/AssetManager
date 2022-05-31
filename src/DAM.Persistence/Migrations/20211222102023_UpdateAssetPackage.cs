using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class UpdateAssetPackage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PackageExtension",
                table: "Assets",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 22, 10, 20, 22, 321, DateTimeKind.Unspecified).AddTicks(8967), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 22, 10, 20, 22, 322, DateTimeKind.Unspecified).AddTicks(6094), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 22, 10, 20, 22, 325, DateTimeKind.Unspecified).AddTicks(7620), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 22, 10, 20, 22, 325, DateTimeKind.Unspecified).AddTicks(9687), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageExtension",
                table: "Assets");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 21, 5, 30, 31, 147, DateTimeKind.Unspecified).AddTicks(6109), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 21, 5, 30, 31, 148, DateTimeKind.Unspecified).AddTicks(2264), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 21, 5, 30, 31, 150, DateTimeKind.Unspecified).AddTicks(6108), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 21, 5, 30, 31, 150, DateTimeKind.Unspecified).AddTicks(7226), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
