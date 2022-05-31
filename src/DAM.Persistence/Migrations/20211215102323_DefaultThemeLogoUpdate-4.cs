using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeLogoUpdate4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "CreatedDate", "Name" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 517, DateTimeKind.Unspecified).AddTicks(409), new TimeSpan(0, 0, 0, 0, 0)), "Simple Asset Manager" });

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Deleted" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 15, 10, 23, 22, 517, DateTimeKind.Unspecified).AddTicks(1545), new TimeSpan(0, 0, 0, 0, 0)), true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 489, DateTimeKind.Unspecified).AddTicks(9132), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 490, DateTimeKind.Unspecified).AddTicks(4909), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "Name" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 492, DateTimeKind.Unspecified).AddTicks(8678), new TimeSpan(0, 0, 0, 0, 0)), "Simple Asset Manager Dark" });

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "Deleted" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 493, DateTimeKind.Unspecified).AddTicks(344), new TimeSpan(0, 0, 0, 0, 0)), false });
        }
    }
}
