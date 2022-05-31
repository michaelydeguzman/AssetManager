using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class ThemeLogoBind : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoFileName",
                table: "Styles",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "Styles",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 2, 5, 45, 59, 65, DateTimeKind.Unspecified).AddTicks(3812), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 2, 5, 45, 59, 66, DateTimeKind.Unspecified).AddTicks(1911), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 2, 5, 45, 59, 69, DateTimeKind.Unspecified).AddTicks(754), new TimeSpan(0, 0, 0, 0, 0)));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoFileName",
                table: "Styles");

            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "Styles");

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 45, DateTimeKind.Unspecified).AddTicks(1512), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 46, DateTimeKind.Unspecified).AddTicks(112), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 1, 5, 25, 12, 49, DateTimeKind.Unspecified).AddTicks(5522), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
