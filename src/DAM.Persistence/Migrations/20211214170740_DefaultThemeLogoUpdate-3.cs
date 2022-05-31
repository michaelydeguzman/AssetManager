using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeLogoUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 492, DateTimeKind.Unspecified).AddTicks(8678), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Themes",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 17, 7, 39, 493, DateTimeKind.Unspecified).AddTicks(344), new TimeSpan(0, 0, 0, 0, 0)), "201.29032258064515,72,91", "245.99999999999997,56,20", "27.835051546391764,0,0" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
                columns: new[] { "CreatedDate", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { new DateTimeOffset(new DateTime(2021, 12, 14, 2, 9, 11, 780, DateTimeKind.Unspecified).AddTicks(5884), new TimeSpan(0, 0, 0, 0, 0)), "246,57,21", "150,100,30", "246,20,41" });
        }
    }
}
