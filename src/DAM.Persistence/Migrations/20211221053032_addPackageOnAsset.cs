using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class addPackageOnAsset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PackageName",
                table: "Assets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PackageUrl",
                table: "Assets",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageName",
                table: "Assets");

            migrationBuilder.DropColumn(
                name: "PackageUrl",
                table: "Assets");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 514, DateTimeKind.Unspecified).AddTicks(239), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 514, DateTimeKind.Unspecified).AddTicks(6230), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 517, DateTimeKind.Unspecified).AddTicks(409), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 517, DateTimeKind.Unspecified).AddTicks(1545), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
