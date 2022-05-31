using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class changeColorName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 11, 23, 1, 41, 6, 482, DateTimeKind.Unspecified).AddTicks(9365), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { "246,57,21", "150,100,30", "246,20,41" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 11, 22, 11, 16, 24, 268, DateTimeKind.Unspecified).AddTicks(3492), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { "#1d1753", "#58547d", "#00aa55" });
        }
    }
}
