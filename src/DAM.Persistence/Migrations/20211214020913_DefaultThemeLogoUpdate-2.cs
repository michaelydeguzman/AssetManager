using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeLogoUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 9, 11, 777, DateTimeKind.Unspecified).AddTicks(2301), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 9, 11, 777, DateTimeKind.Unspecified).AddTicks(7995), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 9, 11, 780, DateTimeKind.Unspecified).AddTicks(4739), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsApplied" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 2, 9, 11, 780, DateTimeKind.Unspecified).AddTicks(5884), new TimeSpan(0, 0, 0, 0, 0)), false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 419, DateTimeKind.Unspecified).AddTicks(5582), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 420, DateTimeKind.Unspecified).AddTicks(1530), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 423, DateTimeKind.Unspecified).AddTicks(8890), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "IsApplied" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 2, 6, 23, 424, DateTimeKind.Unspecified).AddTicks(155), new TimeSpan(0, 0, 0, 0, 0)), true });
        }
    }
}
