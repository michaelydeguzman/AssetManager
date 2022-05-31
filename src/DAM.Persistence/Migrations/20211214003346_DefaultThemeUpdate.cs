using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DAM.Persistence.Migrations
{
    public partial class DefaultThemeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 0);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 515, DateTimeKind.Unspecified).AddTicks(1797), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 515, DateTimeKind.Unspecified).AddTicks(8628), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "IsApplied", "LogoFileName", "LogoUrl", "ModifiedById", "ModifiedDate", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 518, DateTimeKind.Unspecified).AddTicks(4445), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, "Simple Asset Manager Dark", "246,57,21", "150,100,30", "246,20,41" },
                    { 2, null, new DateTimeOffset(new DateTime(2021, 12, 14, 0, 33, 45, 518, DateTimeKind.Unspecified).AddTicks(5440), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, "Simple Asset Manager Light", "246,57,21", "150,100,30", "246,20,41" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Styles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Folders",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 6, 12, 55, 55, 645, DateTimeKind.Unspecified).AddTicks(7742), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Logos",
                keyColumn: "Id",
                keyValue: 0,
                column: "CreatedDate",
                value: new DateTimeOffset(new DateTime(2021, 12, 6, 12, 55, 55, 647, DateTimeKind.Unspecified).AddTicks(2953), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.InsertData(
                table: "Styles",
                columns: new[] { "Id", "CreatedById", "CreatedDate", "Deleted", "IsApplied", "LogoFileName", "LogoUrl", "ModifiedById", "ModifiedDate", "Name", "PrimaryColor", "SecondaryColor", "TertiaryColor" },
                values: new object[] { 0, null, new DateTimeOffset(new DateTime(2021, 12, 6, 12, 55, 55, 653, DateTimeKind.Unspecified).AddTicks(2301), new TimeSpan(0, 0, 0, 0, 0)), false, true, null, null, null, null, "Default", "246,57,21", "150,100,30", "246,20,41" });
        }
    }
}
